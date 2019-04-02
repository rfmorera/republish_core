using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task AddAsync(string GrupoId, string[] links)
        {
            foreach(string st in links)
            {
                try
                {
                    await _dbContext.Set<Anuncio>().AddAsync(new Anuncio() { UrlFormat = new Uri(st), GroupId = GrupoId });
                }
                catch (Exception) { }
                
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllByGroup(string GrupoId)
        {
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.FindAllAsync(p => p.GroupId == GrupoId);
            foreach(Anuncio a in anuncios)
            {
                _dbContext.Set<Anuncio>().Remove(a);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(p => p.Id == Id);
            await repositoryAnuncio.DeleteAsync(anuncio);
        }

        public void Publish(string url)
        {
            throw new NotImplementedException();
        }
    }
}
