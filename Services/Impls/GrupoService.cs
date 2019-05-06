
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

namespace Services.Impls
{
    public class GrupoService : IGrupoService
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Grupo> _repository;
        private readonly Repository<Anuncio> _anuncioRepo;
        private readonly IAnuncioService _anuncioService;
        public GrupoService(ApplicationDbContext context, IAnuncioService anuncioService)
        {
            _context = context;
            _repository = new Repository<Grupo>(_context);
            _anuncioRepo = new Repository<Anuncio>(_context);
            _anuncioService = anuncioService;
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

            IEnumerable<TemporizadorDTO> listT = await (from a in _context.Set<Temporizador>()
                                                        where a.GrupoId == GrupoId
                                                        orderby a.Orden
                                                        select new TemporizadorDTO(a))
                                                  .ToListAsync();
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

        public async Task<IEnumerable<AnuncioDTO>> Select(string GrupoId, int Etapa, string TempNombre)
        {
            try
            {
                IEnumerable<Anuncio> listAnuncio = (await _anuncioRepo.FindAllAsync(a => a.GroupId == GrupoId && a.Actualizado == false))
                                                                      .Take(Etapa);

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
                    listAnuncio = await _anuncioRepo.GetAllAsync();

                    foreach (Anuncio a in listAnuncio)
                    {
                        if (Etapa > 0)
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
            catch (Exception)
            {
                return null;
            }
        }

        public async Task Publish(IEnumerable<AnuncioDTO> list)
        {
            if (list != null)
            {
                string key2Captcha = "a7817c2262085f8d32f276296b3fa669";

                List<Task> anunciosTasks = new List<Task>();
                foreach (AnuncioDTO dTO in list)
                {
                    string url = dTO.Url;
                    anunciosTasks.Add(_anuncioService.Publish(url, key2Captcha));
                }

                await Task.WhenAll(anunciosTasks);
            }
        }
    }
}
