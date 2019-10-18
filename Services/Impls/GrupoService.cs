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
using System.Threading;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Services.Impls
{
    public class GrupoService : IGrupoService
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Grupo> _repository;
        private readonly Repository<Anuncio> _anuncioRepo;
        private readonly IAnuncioService _anuncioService;
        private readonly ITemporizadorService _temporizadorService;
        readonly ILogger<ChequerService> _log;
        private readonly IManejadorFinancieroService _financieroService;

        public GrupoService(ApplicationDbContext context, IAnuncioService anuncioService, ILogger<ChequerService> log, ITemporizadorService temporizadorService, IManejadorFinancieroService financieroService)
        {
            _context = context;
            _repository = new Repository<Grupo>(_context);
            _anuncioRepo = new Repository<Anuncio>(_context);
            _anuncioService = anuncioService;
            _log = log;
            _temporizadorService = temporizadorService;
            _financieroService = financieroService;
        }

        public async Task AddAsync(GrupoIndexDTO grupoDTO)
        {
            Grupo grupo = grupoDTO.BuildModel();
            await _repository.AddAsync(grupo);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Grupo grupo = await _repository.FindAsync(g => g.Id == Id);
            _repository.Remove(grupo);
            await _repository.SaveChangesAsync();
        }

        public async Task<GrupoDetailsDTO> DetailsAsync(string GrupoId)
        {
            Grupo grupo = await _repository.FindAsync(g => g.Id == GrupoId);
            IEnumerable<AnuncioDTO> list = await (from a in _context.Set<Anuncio>()
                                                  where a.GroupId == GrupoId
                                                  orderby a.Orden
                                                  select new AnuncioDTO(a))
                                                  .ToListAsync();

            double costoAnuncio = await _financieroService.CostoAnuncio(grupo.UserId);
            IEnumerable<TemporizadorDTO> listT = (from t in (await _temporizadorService.GetByGroup(GrupoId)).AsQueryable()
                                                select new TemporizadorDTO(t, list.Count(), costoAnuncio)).AsEnumerable();
            
            GrupoDetailsDTO model = new GrupoDetailsDTO(grupo, list, listT);
            return model;
        }

        public async Task<IEnumerable<GrupoIndexDTO>> GetAllAsync(string UserId)
        {
            IEnumerable<GrupoIndexDTO> list = await _repository.QueryAll()
                                                    .Where(g => g.UserId == UserId)
                                                    .OrderBy(g => g.Orden)
                                                    .Include(g => g.Anuncios)
                                                    .Select(g => new GrupoIndexDTO(g))
                                                    .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<Grupo>> GetByUser(string UserId)
        {
            IEnumerable<Grupo> list = await _repository.QueryAll()
                                        .Where(g => g.UserId == UserId)
                                        .OrderBy(g => g.Orden)
                                        .Include(g => g.Anuncios)
                                        .Include(g => g.Temporizadores)
                                        .Select(g => g)
                                        .ToListAsync();
            return list;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AnuncioDTO>> SelectAnuncios(string GrupoId, int Etapa, string TempNombre)
        {
            if(Etapa == 0)
            {
                return (await _anuncioRepo.FindAllAsync(a => a.GroupId == GrupoId)).Select(a => new AnuncioDTO(a));
            }

            IEnumerable<Anuncio> listAnuncio = (await _anuncioRepo.FindAllAsync(a => a.GroupId == GrupoId && a.Actualizado == false));

            if (Etapa > 0)
            {
                listAnuncio = listAnuncio.Take(Etapa);
            }

            List<AnuncioDTO> list = new List<AnuncioDTO>();

            if (listAnuncio.Any())
            {
                foreach (Anuncio a in listAnuncio)
                {
                    a.Actualizado = true;
                    await _anuncioRepo.UpdateAsync(a, a.Id);

                    list.Add(new AnuncioDTO(a));
                }
            }
            else
            {
                listAnuncio = await _anuncioRepo.FindAllAsync(a => a.GroupId == GrupoId);

                foreach (Anuncio a in listAnuncio)
                {
                    if (Etapa > 0 )
                    {
                        list.Add(new AnuncioDTO(a));
                        Etapa--;
                    }
                    else
                    {
                        a.Actualizado = false;
                        await _anuncioRepo.UpdateAsync(a, a.Id);
                    }
                }
            }

            return list;
        }
    }
}
