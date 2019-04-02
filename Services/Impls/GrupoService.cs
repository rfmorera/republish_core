
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;

namespace Services.Impls
{
    public class GrupoService : IGrupoService
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Grupo> _repository;
        public GrupoService(ApplicationDbContext context)
        {
            _context = context;
            _repository = new Repository<Grupo>(_context);
        }

        public async Task AddAsync(GrupoIndexDTO grupoDTO)
        {
            Grupo grupo = grupoDTO.BuildModel();
            await _repository.AddAsync(grupo);
        }

        public async Task DeleteAsync(string Id)
        {
            Grupo grupo = await _repository.FindAsync(g => g.Id == Id);
            await _repository.DeleteAsync(grupo);
        }

        public async Task<GrupoDetailsDTO> DetailsAsync(string GrupoId)
        {
            Grupo grupo = await _repository.FindAsync(g => g.Id == GrupoId);
            IEnumerable<AnuncioDTO> list = await (from a in _context.Set<Anuncio>()
                                                  where a.GroupId == GrupoId
                                                  orderby a.Orden
                                                  select new AnuncioDTO(a))
                                                  .ToListAsync();
            GrupoDetailsDTO model = new GrupoDetailsDTO(grupo, list);
            return model;
        }

        public async Task<IEnumerable<GrupoIndexDTO>> GetAllAsync(string UserId)
        {
            IEnumerable<GrupoIndexDTO> list = await _context.Set<Grupo>()
                                                    .Where(g => g.UserId == UserId)
                                                    .OrderBy(g => g.Orden)
                                                    .Include(g => g.Anuncios)
                                                    .Select(g => new GrupoIndexDTO(g))
                                                    .ToListAsync();
            return list;
        }

        public Task Publish(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
