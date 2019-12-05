using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Services.Utils;
using Microsoft.Extensions.Logging;
using Services.Extensions;
using Services.DTOs.AnuncioHelper;
using Republish.Extensions;

namespace Services.Impls
{
    public class ValidationService : IValidationService
    {
        const short MaxProcesado = 2;
        const short MaxRevalidado = 3;
        private readonly IRepository<Anuncio> _anuncioRepository;
        private readonly INotificationsService _notificationsService;
        private readonly IAnuncioService _anuncioService;
        private readonly IQueueService _queueService;
        readonly ILogger _log;

        public ValidationService(ApplicationDbContext context, INotificationsService notificationsService, ILogger<ValidationService> log, IAnuncioService anuncioService, IQueueService queueService)
        {
            _anuncioRepository = new Repository<Anuncio>(context);
            _notificationsService = notificationsService;
            _log = log;
            _anuncioService = anuncioService;
            _queueService = queueService;
        }

        public async Task<int> VerifyPublication(ICollection<string> link)
        {
            link = link.Distinct().ToList();
            IEnumerable<Anuncio> list = await _anuncioRepository.FindAllAsync(a => link.Contains(a.Id));

            List<Task<bool>> verifyTask = new List<Task<bool>>();
            foreach(Anuncio a in list)
            {
                verifyTask.Add(IsListed(a));
            }
            Task.WaitAll(verifyTask.ToArray());

            int len = list.Count(), cnt = 0;
            for(int i = 0; i < len; i++)
            {
                Anuncio a = list.ElementAt(i);
                if (!verifyTask[i].Result)
                {
                    a.Revalidado += 1;

                    if (a.Revalidado <= MaxRevalidado)
                    {
                        a.Procesando += 1;
                        if (a.Procesando <= MaxProcesado)
                        {
                            await _queueService.Add(a.Id, DateTime.Now);
                        }
                        else
                        {
                            a.Enable = false;
                            _log.LogWarning($"ValidationService Anuncio {a.Id} ha exedido MaxProcesado");
                        }
                    }

                    string message;
                    if (a.Enable.HasValue && !a.Enable.Value)
                    {
                        message = String.Format("Anuncio deshabilitado: ha excedido el máximo de intentos de publicación <a href='{2}' target='_blank' >{0} </a> en la categoría <strong>{1}</strong>.\n", a.Titulo, a.Categoria.ToUpper(), a.Url);
                    }
                    else
                    {
                        message = String.Format("Anuncio escondido: Revolico no está listando el anuncio <a href='{2}' target='_blank' >{0} </a> en la categoría <strong>{1}</strong>.\nContacte a Revolico para que lo habiliten.", a.Titulo, a.Categoria.ToUpper(), a.Url);
                    }
                    await _notificationsService.SendNotification(a.Grupo.UserId, message);
                }
                else
                {
                    a.Procesando = 0;
                    cnt++;
                }
            }

            await _anuncioRepository.SaveChangesAsync();
            return cnt;
        }

        public async Task<bool> IsListed(Anuncio anuncio)
        {
            try
            {
                string anuncioContent = await Requests.GetAsync(anuncio.Url);
                FormUpdateAnuncio formAnuncio = null;
                for(int i = 0; i < 5; i++)
                {
                    try
                    {
                        formAnuncio = _anuncioService.ParseFormAnuncio(anuncioContent);
                        break;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        continue;
                    }
                }

                int cnt = 5;
                string url = $"{Requests.RevolicoBaseUrl}/{formAnuncio.variables.categoria}";
                string urlPage2 = $"{url}pagina-2.html?";
                string html;

                while (cnt > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    html = await Requests.GetAsync(url);
                    if (html.Contains(formAnuncio.variables.title)) return true;

                    html = await Requests.GetAsync(urlPage2);
                    if (html.Contains(formAnuncio.variables.title)) return true;

                    await Task.Delay(TimeSpan.FromSeconds(15));
                    cnt--;
                }

                return false;
            }
            catch(Exception ex)
            {
                _log.LogError(ex.ToExceptionString());
                return false;
            }
        }
    }
}
