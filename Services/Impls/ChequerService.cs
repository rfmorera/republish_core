using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Services.DTOs;
using System.IO;
using Services.Extensions;
using System.Threading;

namespace Services.Impls
{
    public class ChequerService : IChequerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Temporizador> repository;
        private readonly IGrupoService _grupoService;
        public ChequerService(ApplicationDbContext context, IGrupoService grupoService)
        {
            _context = context;
            repository = new Repository<Temporizador>(context);
            _grupoService = grupoService;
        }
        public async Task CheckAllTemporizadores()
        {
            DateTime now = DateTime.Now;
            IEnumerable<Temporizador> list = (await repository.FindAllAsync(t => (
                                                                                        (((t.NextExecution - now) < TimeSpan.FromSeconds(59) && t.NextExecution.Minute == now.Minute)
                                                                                     || (t.NextExecution == t.HoraInicio && t.HoraInicio <= now && now <= t.HoraFin))
                                                                                 && t.IsValidDay()
                                                                                 )));

            //IEnumerable<Temporizador> list = (await repository.FindAllAsync(t => t.Id != null));
            List<Task> publishTasks = new List<Task>();
            foreach (Temporizador t in list)
            {
                TimeSpan timeSpan = TimeSpan.FromHours(t.IntervaloHoras) + TimeSpan.FromMinutes(t.IntervaloMinutos);
                t.NextExecution = now + timeSpan;
                if (t.NextExecution > t.HoraFin)
                {
                    t.NextExecution = t.HoraInicio;
                }
                await repository.UpdateAsync(t, t.Id);
                //await _grupoService.Publish(t.GrupoId, t.Etapa);
                //publishTasks.Add(_grupoService.Publish(t.GrupoId, t.Etapa, t.Nombre));
                new Thread(() =>
                {
                    //Simulate processing            
                    //Thread.SpinWait(Int32.MaxValue / 100);
                    //Console.WriteLine("Emp " + t.Nombre);
                    //Thread.Sleep(new TimeSpan(0, 0, 30));
                    //StreamWriter w = File.AppendText(t.Nombre + ".txt");
                    //await w.WriteAsync("-------------------------------\n\r\n Actualizado : \n"
                    //                    + " actualizado. Por Temporizador: " + t.Nombre + "\nHora Inicio: " + now + ". Hora Fin: " + DateTime.Now.ToLongTimeString() + "\n-------------------------------\n");
                    //w.Close();
                    //Console.WriteLine("Term " + t.Nombre);
                    _grupoService.Publish(t.GrupoId, t.Etapa, "");
                }).Start();
            }
            //await _context.SaveChangesAsync();
            //await Task.WhenAll(publishTasks);

            //StreamWriter w = File.AppendText("log.txt");
            //await w.WriteAsync("Completado Check All\n");
            //w.Close();
        }

        public async Task ResetAll()
        {
            IEnumerable<Temporizador> list = repository.QueryAll().ToList();

            foreach (Temporizador t in list)
            {
                t.NextExecution = t.HoraInicio;
                await repository.UpdateAsync(t, t.Id);
            }
        }
    }
}
