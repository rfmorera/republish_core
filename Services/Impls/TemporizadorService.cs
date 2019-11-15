using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Republish.Extensions;
using Services.Extensions;
using Microsoft.Extensions.Logging;

namespace Services.Impls
{
    public class TemporizadorService : ITemporizadorService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IClienteOpcionesService _opcionesService;
        readonly ILogger<TemporizadorService> _log;
        public TemporizadorService(ApplicationDbContext dbContext, IClienteOpcionesService opcionesService, ILogger<TemporizadorService> log)
        {
            _dbContext = dbContext;
            repositoryTemporizador = new Repository<Temporizador>(dbContext);
            _opcionesService = opcionesService;
            _log = log;
        }

        public async Task AddAsync(Temporizador temporizador, bool SystemEnable)
        {
            temporizador.SystemEnable = SystemEnable;
            temporizador.UserEnable = await _opcionesService.TemporizadorStatus(temporizador.UserId);
            await repositoryTemporizador.AddAsync(temporizador);
            await repositoryTemporizador.SaveChangesAsync();
        }

        public async Task DeleteAllByGroup(string GrupoId)
        {
            IEnumerable<Temporizador> anuncios = await repositoryTemporizador.FindAllAsync(p => p.GrupoId == GrupoId);
            repositoryTemporizador.RemoveRange(anuncios);
            await repositoryTemporizador.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Temporizador temp = await repositoryTemporizador.FindAsync(t => t.Id == Id);
            repositoryTemporizador.Remove(temp);
            await repositoryTemporizador.SaveChangesAsync();
        }

        public async Task<IEnumerable<Temporizador>> GetByGroup(string GrupoId)
        {
            IEnumerable<Temporizador> listT = await  repositoryTemporizador.QueryAll()
                                                                    .Where(t => t.GrupoId == GrupoId)
                                                                    .Include(t => t.Grupo)
                                                                    .OrderBy(r => r.Orden)
                                                                    .ToListAsync();
            return listT;
        }

        public async Task<IEnumerable<Temporizador>> GetByUser(string userId)
        {
            IEnumerable<Temporizador> listT = (await repositoryTemporizador.FindAllAsync(t => t.UserId == userId))
                                                                           .OrderBy(r => r.Orden)
                                                                           .AsEnumerable();
            return listT;
        }

        public async Task SetSystemEnable(string UserId, bool SystemEnable)
        {
            IEnumerable<Temporizador> list = await repositoryTemporizador.FindAllAsync(t => t.UserId == UserId);
            foreach(Temporizador t in list)
            {
                t.SystemEnable = SystemEnable;
                await repositoryTemporizador.UpdateAsync(t, t.Id);
            }
            await repositoryTemporizador.SaveChangesAsync();
        }

        public async Task<Temporizador> TooggleEnable(string Id)
        {
            Temporizador t = await repositoryTemporizador.FindAsync(e => e.Id == Id);
            t.Enable = !t.Enable;
            await repositoryTemporizador.UpdateAsync(t, t.Id);
            await repositoryTemporizador.SaveChangesAsync();
            return t;
        }

        public async Task<bool> ToogleUserEnable(string UserId)
        {
            bool UserEnable = !(await _opcionesService.TemporizadorStatus(UserId));
            await _opcionesService.TemporizadorStatus(UserId, UserEnable);

            IEnumerable<Temporizador> list = await repositoryTemporizador.FindAllAsync(t => t.UserId == UserId && t.UserEnable != UserEnable);
            foreach (Temporizador t in list)
            {
                t.UserEnable = UserEnable;
                await repositoryTemporizador.UpdateAsync(t, t.Id);
            }
            await repositoryTemporizador.SaveChangesAsync();

            return UserEnable;
        }

        /// <summary>
        /// Get All Temporizadores than must be executed. No update the DB
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Temporizador>> GetRunning()
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            TimeSpan utc1 = DateTime.Now.ToUtcCuba().TimeOfDay.Subtract(TimeSpan.FromSeconds(80));
            TimeSpan utcReal = DateTime.Now.ToUtcCuba().TimeOfDay;

            IEnumerable<Temporizador> list = await repositoryTemporizador.QueryAll()
                                                                       .Include(t => t.Grupo)
                                                                       .Where(t => t.SystemEnable
                                                                                && t.UserEnable
                                                                                && t.Enable
                                                                                && t.Grupo.Activo
                                                                                && utc1 <= t.HoraFin
                                                                                && t.NextExecution <= utcReal)
                                                                       .ToListAsync();

            list = list.Where(t => t.IsValidDay(UtcCuba));
            _log.LogWarning(string.Format("Hora {0} cantidad de temporizadores {1}", utcReal.ToString(), list.Count()));
            foreach (Temporizador t in list)
            {
                TimeSpan intervalo = TimeSpan.FromHours(t.IntervaloHoras).Add(TimeSpan.FromMinutes(t.IntervaloMinutos));
                TimeSpan nxT = t.NextExecution.Add(intervalo);
                if (nxT < utcReal)
                {
                    int expectedMin = (int)(utcReal.Subtract(t.HoraInicio)).TotalMinutes;
                    int diff = expectedMin % ((int)intervalo.TotalMinutes);
                    t.NextExecution = utcReal.Subtract(TimeSpan.FromMinutes(diff));
                }

                t.NextExecution = t.NextExecution.Add(intervalo);

                if (t.NextExecution.TotalDays >= 1.0)
                {
                    t.NextExecution = new TimeSpan(23, 59, 55);
                }

                await repositoryTemporizador.UpdateAsync(t, t.Id);
                selectTasks.Add(_grupoService.SelectAnuncios(t.GrupoId, t.Etapa, ""));
            }

            return list;
        }
    }
}
