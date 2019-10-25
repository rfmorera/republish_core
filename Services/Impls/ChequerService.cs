﻿
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Services.DTOs;
using System.IO;
using Services.Extensions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Republish.Extensions;
using Services.Exceptions;
using BlueDot.Data.UnitsOfWorkInterfaces;

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IQueuesUnitOfWork _queuesUnit;
        private readonly IGrupoService _grupoService;
        private readonly IQueueService _queueService;
        private readonly ICaptchaService _captchaService;
        private readonly IRegistroService _registroService;
        private readonly IAnuncioService _anuncioService;
        private readonly IManejadorFinancieroService _financieroService;
        readonly ILogger<ChequerService> _log;

        public ChequerService(ApplicationDbContext context, IGrupoService grupoService, ILogger<ChequerService> log, IQueueService queueService, ICaptchaService captchaService, IRegistroService registroService, IAnuncioService anuncioService, IManejadorFinancieroService financieroService, IQueuesUnitOfWork queuesUnit)
        {
            _context = context;
            repositoryTemporizador = new Repository<Temporizador>(context);
            _queuesUnit = queuesUnit;
            _grupoService = grupoService;
            _log = log;
            _queueService = queueService;
            _captchaService = captchaService;
            _registroService = registroService;
            _anuncioService = anuncioService;
            _financieroService = financieroService;
        }

        public async Task CheckAllTemporizadores()
        {
            try
            {
                DateTime UtcCuba = DateTime.Now.ToUtcCuba();
                TimeSpan utc = DateTime.Now.ToUtcCuba().TimeOfDay;

                IEnumerable<Temporizador> list = await repositoryTemporizador.FindAllAsync(t => t.SystemEnable && t.UserEnable && t.Enable
                                                                              && utc <= t.HoraFin.Add(TimeSpan.FromSeconds(10))
                                                                              && t.NextExecution <= utc.Add(TimeSpan.FromSeconds(10)));
                list = list.Where(t => t.IsValidDay(UtcCuba));

                _log.LogWarning(string.Format("Hora {0} cantidad de temporizadores {1}", utc.ToString(), list.Count()));


                List<Task<IEnumerable<AnuncioDTO>>> selectTasks = new List<Task<IEnumerable<AnuncioDTO>>>();

                foreach (Temporizador t in list)
                {
                    TimeSpan intervalo = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                    TimeSpan nxT = t.NextExecution + intervalo;
                    if (nxT < utc)
                    {
                        int expectedMin = (int)(utc - t.HoraInicio).TotalMinutes;
                        int diff = expectedMin % ((int)intervalo.TotalMinutes);
                        t.NextExecution = utc.Subtract(TimeSpan.FromMinutes(diff));
                    }

                    t.NextExecution += intervalo;
                    await repositoryTemporizador.UpdateAsync(t, t.Id);
                    selectTasks.Add(_grupoService.SelectAnuncios(t.GrupoId, t.Etapa, ""));
                }

                await Task.WhenAll(selectTasks);
                await repositoryTemporizador.SaveChangesAsync();

                List<AnuncioDTO> listAnuncios = new List<AnuncioDTO>();

                int len = selectTasks.Count;
                List<Registro> registros = new List<Registro>(len);
                double costo;
                for (int i = 0; i < len; i++)
                {
                    Task<IEnumerable<AnuncioDTO>> item = selectTasks[i];
                    Temporizador temp = list.ElementAt(i);
                    if (item.IsCompletedSuccessfully && item.Result.Any())
                    {
                        listAnuncios.AddRange(item.Result);
                        _context.Entry(temp).Reference(s => s.Grupo).Load();

                        costo = await _financieroService.CostoAnuncio(temp.Grupo.UserId);
                        int CapResueltos = item.Result.Count();

                        Registro reg = new Registro(temp.Grupo.UserId, CapResueltos, UtcCuba, costo);
                        registros.Add(reg);
                    }
                }

                if (listAnuncios.Any())
                {
                    await _registroService.AddRegistros(registros);

                    _log.LogWarning(string.Format("!!! ---- >>> Queue Messages {0}", listAnuncios.Count()));

                    List<CaptchaKeys> captchaKeys = (await _captchaService.GetCaptchaKeyAsync()).ToList();
                    int idx = 0, lenCaptchas = captchaKeys.Count;
                    List<Task> tasksList = new List<Task>();
                    foreach (AnuncioDTO an in listAnuncios)
                    {
                        tasksList.Add(_anuncioService.Publish(an.Url, captchaKeys[idx].Key));
                        idx = (idx + 1) % lenCaptchas;
                    }

                    int cnt = 0;
                    try
                    {
                        Task.WaitAll(tasksList.ToArray());
                    }
                    catch (AggregateException exs)
                    {
                        foreach (Exception exModel in exs.InnerExceptions)
                        {
                            cnt++;
                            if (exModel is BadCaptchaException)
                            {
                                BadCaptchaException ex = (BadCaptchaException)exModel;
                                _log.LogWarning($"Bad Captcha: {ex.uri}");
                                await _queuesUnit.Short.AddAsync(new ShortQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else if (exModel is BanedException)
                            {
                                BanedException ex = (BanedException)exModel;
                                _log.LogWarning($"Baned Page: {ex.uri} | {ex.Message}");
                                await _queuesUnit.Long.AddAsync(new LongQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else if (exModel is GeneralException)
                            {
                                GeneralException ex = (GeneralException)exModel;
                                _log.LogWarning($"General Error: {ex.uri} | {ex.Message} | {ex.StackTrace}");
                                await _queuesUnit.Long.AddAsync(new LongQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else
                            {
                                Exception ex = exModel;
                                _log.LogWarning($"Unkown Error: {ex.Message} | {ex.StackTrace}");
                            }
                        }
                        await _queuesUnit.SaveChangesAsync();
                    }

                    int totalAnuncios = listAnuncios.Count();
                    int anunciosOk = totalAnuncios - cnt;
                    double pct = 100.0 * anunciosOk / totalAnuncios;

                    _log.LogWarning(string.Format("!!! ---- Actualizados correctamente {0} de {1} | {2}%", anunciosOk, totalAnuncios, pct));
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToExceptionString());
            }
        }

        public async Task ResetAll()
        {
            IEnumerable<Temporizador> list = repositoryTemporizador.QueryAll().ToList();

            foreach (Temporizador t in list)
            {
                t.NextExecution = t.HoraInicio;
                await repositoryTemporizador.UpdateAsync(t, t.Id);
            }
            await repositoryTemporizador.SaveChangesAsync();
        }
    }
}
