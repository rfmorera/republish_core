
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

namespace Services.Impls
{
    public class GrupoService : IGrupoService
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Grupo> _repository;
        private readonly IAnuncioService _anuncioService;
        public GrupoService(ApplicationDbContext context, IAnuncioService anuncioService)
        {
            _context = context;
            _repository = new Repository<Grupo>(_context);
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
            IEnumerable<GrupoIndexDTO> list = await _context.Set<Grupo>()
                                                    .Where(g => g.UserId == UserId)
                                                    .OrderBy(g => g.Orden)
                                                    .Include(g => g.Anuncios)
                                                    .Select(g => new GrupoIndexDTO(g))
                                                    .ToListAsync();
            return list;
        }

        public async Task Publish(string GrupoId)
        {
            try
            {
                IEnumerable<AnuncioDTO> list = await(from a in _context.Set<Anuncio>()
                                                     where a.GroupId == GrupoId
                                                     orderby a.Orden
                                                     select new AnuncioDTO(a))
                                                  .ToListAsync();

                //Estadisticas estadisticas = new Estadisticas();
                //estadisticas.total = _uriList.Count;
                int total = list.Count();
                CountdownEvent countdown = new CountdownEvent(total);

                string key2Captcha = "73894d7e20bf0659da5ea5a15baebf90";

                foreach (AnuncioDTO dTO in list)
                {
                    string url = dTO.Url;
                    ThreadPool.QueueUserWorkItem(state => {
                        try
                        {
                            _anuncioService.Publish(url, key2Captcha);
                            countdown.Signal();
                        }
                        catch (Exception)
                        {

                        }
                    });
                }

                countdown.Wait();

                //for (int i = 0; i < estadisticas.total; i++)
                //{
                //    RevolicoAnuncio anuncio = listaAnuncios[i];
                //    switch (anuncio.estado)
                //    {
                //        case AnuncioEstado.Ok:
                //            estadisticas.ok++;
                //            continue;
                //        case AnuncioEstado.Revolico:
                //            estadisticas.revolico++;
                //            continue;
                //        case AnuncioEstado.InvalidExecution:
                //            estadisticas.otros++;
                //            continue;
                //        case AnuncioEstado.InternetConnection:
                //            estadisticas.internet++;
                //            continue;
                //        case AnuncioEstado.CaptchaError:
                //            estadisticas.captcha++;
                //            continue;
                //        case AnuncioEstado.Undefined:
                //            estadisticas.otros++;
                //            continue;
                //    }
                //}
                //return estadisticas;
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
