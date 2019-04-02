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

        public Task AddAsync(IEnumerable<string> links)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllByGroup(string GrupoId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public void Publish(string url)
        {
            throw new NotImplementedException();
        }
    }
}
