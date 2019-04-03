using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs;

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
        public async Task AddAsync(string GrupoId, TemporizadorDTO temporizadorDTO)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAllByGroup(string GrupoId)
        {
            IEnumerable<Temporizador> anuncios = await repositoryTemporizador.FindAllAsync(p => p.GrupoId == GrupoId);
            foreach (Temporizador a in anuncios)
            {
                _dbContext.Set<Temporizador>().Remove(a);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Temporizador temp = await repositoryTemporizador.FindAsync(t => t.Id == Id);
            await repositoryTemporizador.DeleteAsync(temp);
        }
    }
}
