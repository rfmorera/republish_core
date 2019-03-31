using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Republish.Data.Repositories;

namespace Services.Impls
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Categoria> repository;
        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
            repository = new Repository<Categoria>(context);
        }

        public async Task Add(string UserId, CategoriaIndexDTO categoriaDTO)
        {
            Categoria categoria = categoriaDTO.BuildModel(UserId);
            await repository.AddAsync(categoria);
        }

        public async Task Delete(string Id)
        {
            Categoria categoria = await repository.FindAsync(c => c.Id == Id);
            await repository.DeleteAsync(categoria);
        }

        public async Task<CategoriaDetailsDTO> DetailsAsync(string Id)
        {
            Categoria categoria = await repository.FindAsync(c => c.Id == Id);
            IEnumerable<GrupoIndexDTO> list = await (from g in _context.Set<Grupo>()
                                        where g.CategoriaId == Id
                                        orderby g.Orden
                                        select new GrupoIndexDTO(g))
                                       .ToListAsync();

            CategoriaDetailsDTO detailsDTO = new CategoriaDetailsDTO(categoria, list);
            return detailsDTO;
        }

        public async Task<IEnumerable<CategoriaIndexDTO>> GetAll(string UserId)
        {
            IEnumerable<CategoriaIndexDTO> list = await (from c in _context.Set<Categoria>()
                                                    where c.UserId == UserId
                                                    select new CategoriaIndexDTO(c))
                                                    .ToListAsync();
            return list;
        }

        public Task Update(CategoriaIndexDTO categoriaUpdatedDTO, string Id)
        {
            throw new NotImplementedException();
        }
    }
}
