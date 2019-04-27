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

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repository;
        private readonly IGrupoService _grupoService;
        readonly ILogger<ChequerService> _log;
        public ChequerService(ApplicationDbContext context, IGrupoService grupoService, ILogger<ChequerService> log)
        {
            _context = context;
            repository = new Repository<Temporizador>(context);
            _grupoService = grupoService;
            _log = log;
        }
        public async Task<string> CheckAllTemporizadores()
        {
            string log = "";
            TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            DateTime utc = TimeZoneInfo.ConvertTime(DateTime.Now, hwZone);
            
            IEnumerable<Temporizador> list = (await repository.FindAllAsync(t => (
                                                                                        (((t.NextExecution - utc) < TimeSpan.FromSeconds(59) && t.NextExecution.Minute == utc.Minute)
                                                                                     || (t.NextExecution == t.HoraInicio && t.HoraInicio <= utc && utc <= t.HoraFin))
                                                                                 && t.IsValidDay()
                                                                                 )));

            List<Task> publishTasks = new List<Task>();
            //_log.LogTrace(string.Format("Cantidad de temporizadores {0}", list.Count()), null);
            log += string.Format("Cantidad de temporizadores {0}", list.Count());
            log += "<ul>";
            foreach (Temporizador t in list)
            {
                log += string.Format("<li>{0}<li>", t.Nombre);
                TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                t.NextExecution = utc + timeSpan;
                if (t.NextExecution > t.HoraFin)
                {
                    t.NextExecution = t.HoraInicio;
                }
                await repository.UpdateAsync(t, t.Id);
            }
            log += "</ul>";
            Console.WriteLine(log);
            await _context.SaveChangesAsync();

            List<Task> gruposTasks = new List<Task>();
            foreach (Temporizador t in list)
            {
                gruposTasks.Add(_grupoService.Publish(t.GrupoId, t.Etapa, ""));
            }

            await Task.WhenAll(gruposTasks);

            return log;
        }

        public async Task ResetAll()
        {
            IEnumerable<Temporizador> list = repository.QueryAll().ToList();

            foreach (Temporizador t in list)
            {
                t.NextExecution = t.HoraInicio;
                await repository.UpdateAsync(t, t.Id);
            }
            await _context.SaveChangesAsync();
        }
    }
}
