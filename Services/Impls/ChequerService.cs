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

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repository;
        private readonly IGrupoService _grupoService;
        public ChequerService(ApplicationDbContext context, IGrupoService grupoService)
        {
            _context = context;
            repository = new Repository<Temporizador>(context);
            _grupoService = grupoService;
        }
        public async Task CheckAllTemporizadores()
        {
            DateTime now = DateTime.Now;
            IEnumerable<Temporizador> list = (await repository.FindAllAsync(t => (
                                                                                        (((t.NextExecution - now) < TimeSpan.FromSeconds(59) && t.NextExecution.Minute == now.Minute)
                                                                                     || (t.NextExecution == t.HoraInicio && t.HoraInicio <= now && now <= t.HoraFin))
                                                                                 && t.IsValidDay()
                                                                                 )));

            List<Task> publishTasks = new List<Task>();
            foreach (Temporizador t in list)
            {
                TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                t.NextExecution = now + timeSpan;
                if (t.NextExecution > t.HoraFin)
                {
                    t.NextExecution = t.HoraInicio;
                }
                await repository.UpdateAsync(t, t.Id);
            }

            await _context.SaveChangesAsync();

            foreach (Temporizador t in list)
            {
                new Thread(() =>
                {
                    _grupoService.Publish(t.GrupoId, t.Etapa, "");
                }).Start();
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
        }
    }
}
