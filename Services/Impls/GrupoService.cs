
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
            IEnumerable<GrupoIndexDTO> list = await _context.Set<Grupo>()
                                                    .Where(g => g.UserId == UserId)
                                                    .OrderBy(g => g.Orden)
                                                    .Include(g => g.Anuncios)
                                                    .Select(g => new GrupoIndexDTO(g))
                                                    .ToListAsync();
            return list;
        }

        public async Task Publish(string GrupoId, int Etapa, string TempNombre)
        {
            try
            {
                Random p = new Random();
                int sec = p.Next() % 20;
                string horainicio = DateTime.Now.ToLongTimeString();
                await Task.Delay(sec * 1000);

                //Console.WriteLine(_repository.Find(g => g.Id == GrupoId).Single().Nombre + " actualizado");
                //StreamWriter w = File.AppendText(TempNombre + ".txt");
                //await w.WriteAsync("-------------------------------\n\r\n Actualizado : \n"
                //                    + _repository.Find(g => g.Id == GrupoId).Single().Nombre
                //                    + " actualizado. Por Temporizador: " + TempNombre + " Tiempo " + sec + "segundos. \nHora Inicio: " + horainicio + ". Hora Fin: " + DateTime.Now.ToLongTimeString() + "\n-------------------------------\n");
                //w.Close();
                return;
                IEnumerable<Anuncio> listAnuncio = await (from a in _context.Set<Anuncio>()
                                                    where a.GroupId == GrupoId && a.Actualizado == false
                                                    orderby a.Orden
                                                    select a).Take(Etapa)
                                                  .ToListAsync();
                List<AnuncioDTO> list = new List<AnuncioDTO>();

                foreach (Anuncio a in listAnuncio)
                {
                    a.Actualizado = true;
                    _anuncioRepo.Update(a, a.Id);

                    list.Add(new AnuncioDTO(a));
                }

                if (listAnuncio.Count() < Etapa)
                {
                    listAnuncio = (from a in _context.Set<Anuncio>()
                                   select a).ToList();

                    foreach (Anuncio a in listAnuncio)
                    {
                        a.Actualizado = false;
                        _anuncioRepo.Update(a, a.Id);
                    }
                }

                await _context.SaveChangesAsync();

                int total = list.Count();

                string key2Captcha = "bea50bfde423fb27e7126e873fb42eed";
                List<Task> anunciosTasks = new List<Task>();
                foreach (AnuncioDTO dTO in list)
                {
                    string url = dTO.Url;
                    anunciosTasks.Add(_anuncioService.Publish(url, key2Captcha));
                }

                await Task.WhenAll(anunciosTasks);
            }
            catch (Exception)
            {

            }
        }
    }
}
