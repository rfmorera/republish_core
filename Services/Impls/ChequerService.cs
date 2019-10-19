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

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repository;
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
            repository = new Repository<Temporizador>(context);
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

                IEnumerable<Temporizador> list = await repository.FindAllAsync(t => t.SystemEnable && t.UserEnable  && t.Enable && utc <= t.HoraFin && t.NextExecution <= utc);
                list = list.Where(t => t.IsValidDay(UtcCuba));

                _log.LogWarning(string.Format("Hora {0} cantidad de temporizadores {1}", utc.ToString(), list.Count()));
                
                List<Task<IEnumerable<AnuncioDTO>>> selectTasks = new List<Task<IEnumerable<AnuncioDTO>>>();

                foreach (Temporizador t in list)
                {
                    TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                    t.NextExecution = utc + timeSpan;
                    await repository.UpdateAsync(t, t.Id);

                    selectTasks.Add(_grupoService.SelectAnuncios(t.GrupoId, t.Etapa, ""));
                }

                await Task.WhenAll(selectTasks);
                await repository.SaveChangesAsync();

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
                    List<Task<string>> tasksList = new List<Task<string>>();
                    foreach(AnuncioDTO an in listAnuncios)
                    {
                        tasksList.Add(_anuncioService.Publish(an.Url, captchaKeys[idx].Key));
                        idx = (idx + 1) % lenCaptchas;
                    }
                    //await _queueService.AddMessageAsync(KeyCaptcha, listAnuncios);
                    await Task.WhenAll(tasksList);
                    int cnt = 0;
                    foreach(Task<string> ans in tasksList)
                    {
                        if (!String.IsNullOrEmpty(ans.Result))
                        {
                            cnt++;
                            _log.LogError("Bad update> " + ans.Result);
                        }
                    }

                    _log.LogWarning(string.Format("!!! ---- Actualizados correctamente {0}", listAnuncios.Count() - cnt));
                }
            }
            catch(Exception ex)
            {
                _log.LogError(ex.ToExceptionString());
            }
        }

        public async Task ResetAll()
        {
            IEnumerable<Temporizador> list = repository.QueryAll().ToList();

            foreach (Temporizador t in list)
            {
                t.NextExecution = t.HoraInicio;
                await repository.UpdateAsync(t, t.Id);
            }
            await repository.SaveChangesAsync();
        }
    }
}
