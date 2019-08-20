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

namespace Services.Impls
{
    public class TemporizadorService : ITemporizadorService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Temporizador> repositoryTemporizador;
        public TemporizadorService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryTemporizador = new Repository<Temporizador>(dbContext);
        }
        public async Task AddAsync(Temporizador temporizador, bool SystemEnable)
        {
            temporizador.SystemEnable = SystemEnable;
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
            IEnumerable<Temporizador> listT = (await repositoryTemporizador.FindAllAsync(t => t.GrupoId == GrupoId))
                                                                           .OrderBy(r => r.Orden)
                                                                           .AsEnumerable();
            return listT;
        }

        public async Task SetEnable(string UserId, bool SystemEnable)
        {
            IEnumerable<Temporizador> list = await repositoryTemporizador.FindAllAsync(t => t.UserId == UserId);
            foreach(Temporizador t in list)
            {
                t.SystemEnable = SystemEnable;
                await repositoryTemporizador.UpdateAsync(t, t.Id);
            }
            await repositoryTemporizador.SaveChangesAsync();
        }
    }
}
