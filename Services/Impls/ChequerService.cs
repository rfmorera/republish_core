using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using System.IO;
using Services.Extensions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Republish.Extensions;
using Services.Exceptions;
using BlueDot.Data.UnitsOfWorkInterfaces;
using System.Net;
using Services.Results;

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IGrupoService _grupoService;
        private readonly ICaptchaService _captchaService;
        private readonly IRegistroService _registroService;
        private readonly IAnuncioService _anuncioService;
        private readonly IManejadorFinancieroService _financieroService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly IValidationService _validationService;
        readonly ILogger<ChequerService> _log;

        public ChequerService(ApplicationDbContext context, IGrupoService grupoService, ILogger<ChequerService> log, ICaptchaService captchaService, IRegistroService registroService, IAnuncioService anuncioService, IManejadorFinancieroService financieroService, ITemporizadorService temporizadorService, IValidationService validationService)
        {
            _context = context;
            repositoryTemporizador = new Repository<Temporizador>(context);
            _grupoService = grupoService;
            _log = log;
            _captchaService = captchaService;
            _registroService = registroService;
            _anuncioService = anuncioService;
            _financieroService = financieroService;
            _temporizadorService = temporizadorService;
            _validationService = validationService;
        }

        public async Task CheckAllTemporizadores()
        {
            try
            {
                DateTime UtcCuba = DateTime.Now.ToUtcCuba();
                IEnumerable<Temporizador> list = await _temporizadorService.GetRunning();
                
                List<Task<IEnumerable<Anuncio>>> getAnunciosTasks = new List<Task<IEnumerable<Anuncio>>>();

                foreach (Temporizador t in list)
                {
                    getAnunciosTasks.Add(_grupoService.GetAnunciosToUpdate(t.GrupoId, t.Etapa));
                }

                await Task.WhenAll(getAnunciosTasks);
                
                List<Anuncio> listAnuncios = new List<Anuncio>();
                int len = getAnunciosTasks.Count;
                List<Registro> registros = new List<Registro>(len);
                double costo;
                for (int i = 0; i < len; i++)
                {
                    Task<IEnumerable<Anuncio>> item = getAnunciosTasks[i];
                    Temporizador temp = list.ElementAt(i);
                    if (item.IsCompletedSuccessfully && item.Result.Any())
                    {
                        listAnuncios.AddRange(item.Result);

                        costo = await _financieroService.CostoAnuncio(temp.UserId);
                        int CapResueltos = item.Result.Count();

                        Registro reg = new Registro(temp.UserId, CapResueltos, UtcCuba, costo);
                        registros.Add(reg);
                    }
                }

                if (listAnuncios.Any())
                {
                    await _registroService.AddRegistros(registros);

                    _log.LogWarning(string.Format("!!! ---- >>> Queue Messages {0}", listAnuncios.Count()));

                    List<CaptchaKeys> captchaKeys = (await _captchaService.GetCaptchaKeyAsync()).ToList();
                    int idx = 0, lenCaptchas = captchaKeys.Count;
                    List<Task<ReinsertResult>> reinsertTask = new List<Task<ReinsertResult>>();
                    foreach (Anuncio an in listAnuncios)
                    {
                        reinsertTask.Add(_anuncioService.ReInsert(an, captchaKeys[idx].Key));
                        idx = (idx + 1) % lenCaptchas;
                    }

                    await Task.WhenAll(reinsertTask);

                    List<string> anunciosEliminados = new List<string>();
                    List<Anuncio> anunciosProcesados = new List<Anuncio>();
                    foreach (Task<ReinsertResult> taskResult in reinsertTask)
                    {
                        ReinsertResult result = taskResult.Result;
                        if (taskResult.IsCompletedSuccessfully && taskResult.Result.Success)
                        {
                            anunciosProcesados.Add(result.Anuncio);
                        }
                        else
                        {
                            if (result.HasException)
                            {
                                _log.LogWarning($"{result.Anuncio.GetUriId} | {result.Exception.Message} | {result.Exception.StackTrace}");
                            }
                            if (result.IsDeleted)
                            {
                                anunciosEliminados.Add(result.Anuncio.Id);
                            }
                            // Add to requeue
                        }
                    }

                    await _anuncioService.NotifyDelete(anunciosEliminados);
                    await _anuncioService.Update(anunciosProcesados);

                    int totalProcesados = anunciosProcesados.Count;
                    int totalAnuncios = listAnuncios.Count();
                    double pct = 100.0 * totalProcesados / totalAnuncios;
                    _log.LogWarning(string.Format("!!! ---- Actualizados correctamente {0} de {1} | {2}%", totalProcesados, totalAnuncios, pct));

                    int verifyPub = await _validationService.VerifyPublication(anunciosProcesados.Select(a => a.Url).ToList());
                    double pctVerify = 100.0 * verifyPub / totalAnuncios;
                    _log.LogWarning(string.Format("!!! ---- Mostrados correctamente {0} de {1} | {2}%", verifyPub, totalAnuncios, pct));
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
