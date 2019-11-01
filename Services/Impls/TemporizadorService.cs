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

namespace Services.Impls
{
    public class TemporizadorService : ITemporizadorService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        private readonly IClienteOpcionesService _opcionesService;
        public TemporizadorService(ApplicationDbContext dbContext, IClienteOpcionesService opcionesService)
        {
            _dbContext = dbContext;
            repositoryTemporizador = new Repository<Temporizador>(dbContext);
            _opcionesService = opcionesService;
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
    }
}
