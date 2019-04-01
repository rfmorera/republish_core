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
        private readonly IGrupoService _grupoService;
        public CategoriaService(ApplicationDbContext context, IGrupoService grupoService)
        {
            _context = context;
            repository = new Repository<Categoria>(context);
            _grupoService = grupoService;
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

            IEnumerable<GrupoIndexDTO> list = await _grupoService.GetAllAsync(Id);

            CategoriaDetailsDTO detailsDTO = new CategoriaDetailsDTO(categoria, list);
            return detailsDTO;
        }

        public async Task<IEnumerable<CategoriaIndexDTO>> GetAllAsync(string UserId)
        {
            IEnumerable<CategoriaIndexDTO> list = await (from c in _context.Set<Categoria>()
                                                    where c.UserId == UserId
                                                    orderby c.Orden
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
