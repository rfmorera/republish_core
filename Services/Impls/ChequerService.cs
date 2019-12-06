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
using Microsoft.EntityFrameworkCore.Storage;

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IRepository<RemoveQueue> _removeRepository;
        private readonly IGrupoService _grupoService;
        private readonly ICaptchaService _captchaService;
        private readonly IRegistroService _registroService;
        private readonly IAnuncioService _anuncioService;
        private readonly IManejadorFinancieroService _financieroService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly IValidationService _validationService;
        private readonly IEmailRandomService _emailRandomService;
        private readonly IQueueService _queueService;
        readonly ILogger<ChequerService> _log;

        public ChequerService(ApplicationDbContext context, IGrupoService grupoService, ILogger<ChequerService> log, ICaptchaService captchaService, IRegistroService registroService, IAnuncioService anuncioService, IManejadorFinancieroService financieroService, ITemporizadorService temporizadorService, IValidationService validationService, IEmailRandomService emailRandomService, IQueueService queueService)
        {
            _context = context;
            repositoryTemporizador = new Repository<Temporizador>(context);
            _removeRepository = new Repository<RemoveQueue>(context);
            _grupoService = grupoService;
            _log = log;
            _captchaService = captchaService;
            _registroService = registroService;
            _anuncioService = anuncioService;
            _financieroService = financieroService;
            _temporizadorService = temporizadorService;
            _validationService = validationService;
            _emailRandomService = emailRandomService;
            _queueService = queueService;
        }

        public async Task CheckAllTemporizadores()
        {
            try
            {
                DateTime UtcCuba = DateTime.Now.ToUtcCuba();
                TimeSpan ini = new TimeSpan(0, 15, 0);
                TimeSpan fin = new TimeSpan(23, 50, 0);
                if (ini > UtcCuba.TimeOfDay || UtcCuba.TimeOfDay > fin)
                {
                    return;
                }

                IEnumerable<Temporizador> list = await _temporizadorService.GetRunning();
                
                List<Task<IEnumerable<Anuncio>>> getAnunciosTasks = new List<Task<IEnumerable<Anuncio>>>();

                foreach (Temporizador t in list)
                {
                    getAnunciosTasks.Add(_anuncioService.GetAnunciosToUpdate(t.GrupoId, t.Etapa));
                }

                await Task.WhenAll(getAnunciosTasks);
                await SaveChanges();

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

                IEnumerable<Anuncio> anunciosFromQueue = await _queueService.GetAnunciosFromQueue();

                listAnuncios.AddRange(anunciosFromQueue);

                listAnuncios = listAnuncios.GroupBy(a => a.GetUriId).Select(b => b.First()).ToList();
                if (listAnuncios.Any())
                {
                    await _registroService.AddRegistros(registros);

                    _log.LogWarning(string.Format("!!! ---- >>> Queue Messages {0}", listAnuncios.Count()));

                    List<CaptchaKeys> captchaKeys = (await _captchaService.GetCaptchaKeyAsync()).ToList();
                    List<Emails> randomEmails = (await _emailRandomService.GetList()).ToList();

                    int idxCaptcha = 0, 
                        idxEmail = (new Random(DateTime.Now.Millisecond)).Next(0, randomEmails.Count), 
                        lenCaptchas = captchaKeys.Count,
                        lenEmails = randomEmails.Count,
                        cntAnuncios = 0;
                    List<Task<ReinsertResult>> reinsertTask = new List<Task<ReinsertResult>>();
                    foreach (Anuncio an in listAnuncios)
                    {
                        cntAnuncios++;
                        reinsertTask.Add(_anuncioService.ReInsert(an, captchaKeys[idxCaptcha].Key, randomEmails[idxEmail].Email));
                        idxCaptcha = (idxCaptcha + 1) % lenCaptchas;
                        idxEmail = (idxEmail + 1) % lenEmails;

                        if(cntAnuncios == 30)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(30));
                            cntAnuncios = 0;
                        }
                    }

                    await Task.WhenAll(reinsertTask);

                    List<Anuncio> anunciosProcesados = new List<Anuncio>(),
                                  anunciosEliminados = new List<Anuncio>();
                    len = reinsertTask.Count;
                    for (int i = 0; i < len; i++)
                    {
                        Task<ReinsertResult> taskResult = reinsertTask[i];
                        ReinsertResult result = taskResult.Result;
                        if (taskResult.IsCompletedSuccessfully && taskResult.Result.Success)
                        {
                            Anuncio an = result.Anuncio;
                            an.Procesando = 0;
                            anunciosProcesados.Add(an);
                        }
                        else
                        {
                            DateTime dateTime = DateTime.Now.ToUtcCuba();
                            if (result.HasException)
                            {
                                _log.LogWarning($"{result.Anuncio.GetUriId} | {result.Exception.Message} | {result.Exception.StackTrace}");
                            }

                            if (result.IsBaned)
                            {
                                dateTime = dateTime.AddMinutes(2);
                            }

                            if(result.NonRemoved)
                            {
                                int pos = result.Exception.Message.IndexOf("https");
                                string url = result.Exception.Message.Substring(pos);
                                await _removeRepository.AddAsync(new RemoveQueue() { Url = url});
                                Anuncio an = result.Anuncio;
                                an.Procesando = 0;
                                anunciosProcesados.Add(an);
                                continue;
                            }

                            if (result.IsDeleted)
                            {
                                Anuncio an = result.Anuncio;
                                an.Procesando = 0;
                                an.Enable = false;
                                an.Eliminado = true;
                                anunciosEliminados.Add(result.Anuncio);
                                continue;
                            }

                            if (result.Despublicado)
                            {
                                Anuncio an = result.Anuncio;
                                an.Procesando = 0;
                                an.Enable = false;
                                an.Despublicado = true;
                                anunciosEliminados.Add(result.Anuncio);
                                continue;
                            }

                            if (!result.BadCaptcha)
                            {
                                Anuncio an = result.Anuncio;
                                an.Procesando = 0;
                                continue;
                            }

                            await _queueService.Add(result.Anuncio.Id, dateTime);
                        }
                    }

                    await SaveChanges();
                    await _anuncioService.NotifyDelete(anunciosEliminados);

                    int totalProcesados = anunciosProcesados.Count;
                    int totalAnuncios = listAnuncios.Count();
                    double pct = 100.0 * totalProcesados / totalAnuncios;
                    _log.LogWarning(string.Format("!!! ---- Actualizados correctamente {0} de {1} | {2}%", totalProcesados, totalAnuncios, pct));

                    //int verifyPub = await _validationService.VerifyPublication(anunciosProcesados.Select(a => a.Id).ToList());
                    //double pctVerify = 100.0 * verifyPub / totalProcesados;
                    //_log.LogWarning(string.Format("!!! ---- Mostrados correctamente {0} de {1} | {2}%", verifyPub, totalProcesados, pct));
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
            await _anuncioService.Reset();
            await repositoryTemporizador.SaveChangesAsync();
        }

        private async Task SaveChanges()
        {
            int saved = 5;
            while (saved > 0)
            {
                try
                {
                    // Attempt to save changes to the database
                    await _context.SaveChangesAsync();
                    saved--;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    int len = ex.Entries.Count;
                    for (int i = 0; i < len; i++)
                    {
                        var entry = ex.Entries[i];
                        var clientValues = (Anuncio)entry.Entity;
                        var databaseEntry = entry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            _log.LogInformation("Unable to save changes. The anuncio  was deleted by another user.");
                        }
                        else
                        {
                            Anuncio databaseValues = (Anuncio)databaseEntry.ToObject();

                            if (databaseValues.Url != clientValues.Url)
                            {
                                _log.LogError($"Conflicto de URL > {clientValues.Id} | DB {databaseValues.Url} Client {clientValues.Url}");
                                databaseValues.Url = clientValues.Url;
                            }
                            else
                            {
                                _log.LogError("Don't know how to handle concurrency conflicts for " + clientValues.Id);
                            }
                            entry.OriginalValues.SetValues(databaseEntry);
                        }
                    }   
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    _log.LogError("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return;
                }
            }
        }

        public async Task ResetRemoveQueue()
        {
            List<RemoveQueue> list = _removeRepository.QueryAll().ToList();
            List<string> urlList = list.Select(e => e.Url).ToList();
            _removeRepository.RemoveRange(list);
            await _removeRepository.SaveChangesAsync();

            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (string t in urlList)
            {
                tasks.Add(_anuncioService.DeleteFromRevolico(t));
            }
            Task.WaitAll(tasks.ToArray());

            int len = list.Count;
            for(int i = 0; i < len; i++)
            {
                if(tasks[i].IsFaulted)
                {
                    await _removeRepository.AddAsync(new RemoveQueue() { Url = urlList[i] });
                }
            }
            await _removeRepository.SaveChangesAsync();
        }
    }
}
