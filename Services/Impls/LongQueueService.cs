using Microsoft.Extensions.Logging;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Models;
using Services.Exceptions;
using BlueDot.Data.UnitsOfWorkInterfaces;
using Microsoft.EntityFrameworkCore;
using Republish.Extensions;

namespace Services.Impls
{
    public class LongQueueService : ILongQueueService
    {
        private readonly IAnuncioService _anuncioService;
        private readonly IQueuesUnitOfWork _queuesUnit;
        private readonly ICaptchaService _captchaService;
        readonly ILogger<ChequerService> _log;

        public LongQueueService(ApplicationDbContext context, ILogger<ChequerService> log, IAnuncioService anuncioService, IQueuesUnitOfWork queuesUnit, ICaptchaService captchaService)
        {
            _queuesUnit = queuesUnit;
            _log = log;
            _anuncioService = anuncioService;
            _captchaService = captchaService;
        }

        public async Task Clean()
        {
            _queuesUnit.Short.RemoveRange(await _queuesUnit.Short.QueryAll().ToListAsync());
            await _queuesUnit.SaveChangesAsync();
        }

        public async Task Process()
        {
            try
            {
                IEnumerable<string> list = _queuesUnit.Long.QueryAll()
                                                            .OrderBy(l => l.Created)
                                                            .GroupBy(l => l.Url)
                                                            .OrderBy(l => l.Key)
                                                            .Take(20)
                                                            .Select(l => l.Key);

                IEnumerable<LongQueue> usedLinks = _queuesUnit.Long.QueryAll()
                                                                     .Join(list,
                                                                           r => r.Url,
                                                                           s => s,
                                                                           (a, b) => a);

                DateTime UtcCuba = DateTime.Now.ToUtcCuba();
                if (list.Any())
                {
                    List<Task> tasksList = new List<Task>();
                    List<CaptchaKeys> captchaKeys = (await _captchaService.GetCaptchaKeyAsync()).ToList();
                    int idx = 0, lenCaptchas = captchaKeys.Count;
                    foreach (string item in list)
                    {
                        tasksList.Add(_anuncioService.Publish(item, captchaKeys[idx].Key));
                        idx = (idx + 1) % lenCaptchas;
                    }
                    int totalAnuncios = list.Count();

                    _queuesUnit.Long.RemoveRange(usedLinks);
                    await _queuesUnit.SaveChangesAsync();

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
                                _log.LogWarning($"Long Queue > Bad Captcha: {ex.uri} | {ex.Message}");
                                await _queuesUnit.Short.AddAsync(new ShortQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else if (exModel is BanedException)
                            {
                                BanedException ex = (BanedException)exModel;
                                _log.LogWarning($"Long Queue > Baned Page: {ex.uri}");
                                await _queuesUnit.Long.AddAsync(new LongQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else if (exModel is GeneralException)
                            {
                                GeneralException ex = (GeneralException)exModel;
                                _log.LogWarning($"Long Queue > Custom Error: {ex.uri} | {ex.Message} | {ex.StackTrace}");
                                await _queuesUnit.Long.AddAsync(new LongQueue() { Url = ex.uri, Created = UtcCuba });
                            }
                            else
                            {
                                Exception ex = exModel;
                                _log.LogWarning($"Long Queue > Unkown Error: {ex.Message} | {ex.StackTrace}");
                            }
                        }
                    }
                    await _queuesUnit.SaveChangesAsync();

                    int anunciosOk = totalAnuncios - cnt;
                    double pct = 100.0 * anunciosOk / totalAnuncios;

                    _log.LogWarning(string.Format("!!! Long Queue ---- Actualizados correctamente {0} de {1} | {2}%", anunciosOk, totalAnuncios, pct));
                }
            }
            catch(Exception ex)
            {
                _log.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            
        }
    }
}
