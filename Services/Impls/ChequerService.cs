﻿using Models;
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

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IRepository<ShortQueue> repositoryShortQueue;
        private readonly IRepository<LongQueue> repositoryLongQueue;
        private readonly IGrupoService _grupoService;
        private readonly IQueueService _queueService;
        private readonly ICaptchaService _captchaService;
        private readonly IRegistroService _registroService;
        private readonly IAnuncioService _anuncioService;
        private readonly IManejadorFinancieroService _financieroService;
        readonly ILogger<ChequerService> _log;

        public ChequerService(ApplicationDbContext context, IGrupoService grupoService, ILogger<ChequerService> log, IQueueService queueService, ICaptchaService captchaService, IRegistroService registroService, IAnuncioService anuncioService, IManejadorFinancieroService financieroService)
        {
            _context = context;
            repositoryTemporizador = new Repository<Temporizador>(context);
            repositoryShortQueue = new Repository<ShortQueue>(context);
            repositoryLongQueue = new Repository<LongQueue>(context);
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
                                                                              && utc <= t.HoraFin + TimeSpan.FromSeconds(11) 
                                                                              && t.NextExecution <= utc);
                list = list.Where(t => t.IsValidDay(UtcCuba));

                _log.LogWarning(string.Format("Hora {0} cantidad de temporizadores {1}", utc.ToString(), list.Count()));
                
                List<Task<IEnumerable<AnuncioDTO>>> selectTasks = new List<Task<IEnumerable<AnuncioDTO>>>();

                foreach (Temporizador t in list)
                {
                    TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                    t.NextExecution += timeSpan;
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
                    foreach(AnuncioDTO an in listAnuncios)
                    {
                        tasksList.Add(_anuncioService.Publish(an.Url, captchaKeys[idx].Key));
                        idx = (idx + 1) % lenCaptchas;
                    }
                    
                    await Task.WhenAll(tasksList);
                    int cnt = 0;
                    foreach(Task ans in tasksList)
                    {
                        if (ans.IsFaulted)
                        {
                            cnt++;
                            if(ans.Exception.InnerException is BadCaptchaException)
                            {
                                BadCaptchaException ex = (BadCaptchaException) ans.Exception.InnerException;
                                _log.LogWarning($"Bad Captcha: {ex.uri} | {ex.Message}");
                                await repositoryShortQueue.AddAsync(new ShortQueue() { Url = ex.uri});
                            }
                            else if (ans.Exception.InnerException is BanedException)
                            {
                                BanedException ex = (BanedException)ans.Exception.InnerException;
                                _log.LogWarning($"Baned Page: {ex.uri} | {ex.Message} | {ex.StackTrace}");
                                await repositoryLongQueue.AddAsync(new LongQueue() { Url = ex.uri });
                            }
                            else if (ans.Exception.InnerException is GeneralException)
                            {
                                GeneralException ex = (GeneralException) ans.Exception.InnerException;
                                _log.LogWarning($"Custom Error: {ex.uri} | {ex.Message} | {ex.StackTrace}");
                                await repositoryShortQueue.AddAsync(new ShortQueue() { Url = ex.uri });
                            }
                            else 
                            {
                                Exception ex = ans.Exception.InnerException;
                                _log.LogWarning($"Unkown Error: {ex.Message} | {ex.StackTrace}");
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    int totalAnuncios = listAnuncios.Count();
                    int anunciosOk = totalAnuncios - cnt;
                    double pct = 100.0 * anunciosOk / totalAnuncios;

                    _log.LogWarning(string.Format("!!! ---- Actualizados correctamente {0} de {1} | {2}%", anunciosOk, totalAnuncios, pct));
                }
            }
            catch(Exception ex)
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
