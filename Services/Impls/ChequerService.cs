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
            TimeSpan utc = DateTime.Now.ToUtcCuba().TimeOfDay;
            IEnumerable<Temporizador> list = await repository.FindAllAsync(t => utc <= t.HoraFin && t.NextExecution <= utc );

            list = list.Where(t => t.IsValidDay());

            List<Task<IEnumerable<AnuncioDTO>>> selectTasks = new List<Task<IEnumerable<AnuncioDTO>>>();
            foreach (Temporizador t in list)
            {
                TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                t.NextExecution = utc + timeSpan;
                if (t.NextExecution > t.HoraFin)
                {
                    t.NextExecution = t.HoraInicio;
                }
                await repository.UpdateAsync(t, t.Id);

                selectTasks.Add(_grupoService.Select(t.GrupoId, t.Etapa, ""));
            }

            await Task.WhenAll(selectTasks);
            await repository.SaveChangesAsync();

            List<Task> publishTasks = new List<Task>();
            foreach (Task<IEnumerable<AnuncioDTO>> item in selectTasks)
            {
                publishTasks.Add(_grupoService.Publish(item.Result));
            }

            await Task.WhenAll(publishTasks);

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
            await repository.SaveChangesAsync();
        }
    }
}
