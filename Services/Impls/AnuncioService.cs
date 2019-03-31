using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        public AnuncioService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
        }

        public async Task<IEnumerable<Anuncio>> AddAsync(IEnumerable<Anuncio> anuncios)
        {
            await repositoryAnuncio.AddAllAsync(anuncios);
            return anuncios;
        }

        public async Task<int> RemoveByIdAsync(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(p => p.Id == Id);
            return await repositoryAnuncio.DeleteAsync(anuncio);
        }

        public void Publish(string url)
        {
            
        }
    }
}
