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
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Threading;
using HtmlAgilityPack;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Services.Extensions;
using Captcha2Api;
using Services.Utils;
using Services.DTOs.AnuncioHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Exceptions;
using Services.DTOs;
using Republish.Extensions;

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private const string apiRevolico = "https://api.revolico.com/graphql/";

        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        private readonly INotificationsService _notificationsService;
        readonly ILogger _log;

        public AnuncioService(ILogger log, INotificationsService notificationsService)
        {
            _log = log;
            _notificationsService = notificationsService;
        }

        public AnuncioService(ApplicationDbContext dbContext, ILogger<AnuncioService> log)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
            _log = log;
        }

        public async Task AddAsync(string GrupoId, string[] links)
        {
            foreach (string st in links)
            {
                try
                {
                    Anuncio anuncio = new Anuncio() { UrlFormat = new Uri(st), GroupId = GrupoId };
                    repositoryAnuncio.Add(anuncio);
                }
                catch (Exception) { }

            }
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task DeleteAllByGroup(string GrupoId)
        {
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.FindAllAsync(p => p.GroupId == GrupoId);
            repositoryAnuncio.RemoveRange(anuncios);

            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(p => p.Id == Id);
            repositoryAnuncio.Remove(anuncio);
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task DeleteAsync(List<string> list)
        {
            IEnumerable<Anuncio> anuncios = (await repositoryAnuncio.FindAllAsync(a => list.Contains(a.Url))).AsEnumerable();
            repositoryAnuncio.RemoveRange(anuncios);
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<Anuncio> anuncios)
        {
            repositoryAnuncio.RemoveRange(anuncios);
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task NotifyDelete(List<string> list)
        {
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.QueryAll()
                                                                    .Where(a => list.Contains(a.Url))
                                                                    .Include(a => a.Grupo)
                                                                    .ToListAsync();
            List<Notificacion> notificacions = new List<Notificacion>();
            foreach (Anuncio item in anuncios)
            {
                notificacions.Add(new Notificacion()
                {
                    UserId = item.Grupo.UserId,
                    DateCreated = DateTime.Now.ToUtcCuba(),
                    Mensaje = String.Format("Del grupo {1} el anuncio {0} a caducado/eliminado por tanto se ha eliminado del sistema: {0}", item.Url, item.Grupo.Nombre),
                    Readed = false
                });
            }

            await _notificationsService.Add(notificacions);

            await DeleteAsync(anuncios);
        }

        public async Task Publish(string url, string Key2Captcha)
        {
            await StartProcess(url, Key2Captcha, true);
        }

        private async Task StartProcess(string _uri, string key2captcha, bool v2)
        {
            try
            {
                string htmlAnuncio = await Requests.GetAsync(_uri);
                GetException(htmlAnuncio, _uri, false);

                FormAnuncio formAnuncio = ParseFormAnuncio(htmlAnuncio);

                CaptchaAnswer captchaResponse = await ResolveCaptcha(key2captcha, _uri, htmlAnuncio);

                formAnuncio.variables.captchaResponse = captchaResponse.Answer;
                string jsonForm = $"[{JsonConvert.SerializeObject(formAnuncio)}]";

                string answer = await Requests.PostAsync(apiRevolico, jsonForm);

                GetException(answer, _uri, true, captchaResponse);
                //_captchaSolver.set_captcha_good(captchaResponse.Id);
            }
            catch (BadCaptchaException ex)
            {
                throw ex;
            }
            catch (BanedException ex)
            {
                throw ex;
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (GeneralException ex)
            {
                ex.uri = _uri;
                throw ex;
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message + "\n" + ex.StackTrace, _uri);
            }
        }

        private async Task<CaptchaAnswer> ResolveCaptcha(string key2captcha, string _uri, string htmlAnuncio)
        {
            int p1 = htmlAnuncio.IndexOf("RECAPTCHA_V2_SITE_KEY") + "RECAPTCHA_V2_SITE_KEY".Length + 3;
            int p2 = htmlAnuncio.IndexOf("RECAPTCHA_V3_SITE_KEY") - 3;

            string siteKey = htmlAnuncio.Substring(p1, p2 - p1);
            //string siteKey = "6LfyRCIUAAAAAP5zhuXfbwh63Sx4zqfPmh3Jnjy7";
            string captchaId = await Captcha2Solver.submit_recaptcha(key2captcha, _uri, siteKey);
            WebException last = null;

            await Task.Delay(15000);
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    string ans = await Captcha2Solver.retrieve(key2captcha, captchaId);
                    if (!String.IsNullOrEmpty(ans))
                    {
                        return new CaptchaAnswer(key2captcha, captchaId, ans);
                    }
                    await Task.Delay(10000);
                }
                catch (WebException ex)
                {
                    last = ex;
                }
            }
            if (last != null)
            {
                throw last;
            }
            throw new BadCaptchaException("ERROR_CAPTCHA_UNSOLVABLE", _uri);
        }

        private FormAnuncio ParseFormAnuncio(string htmlAnuncio)
        {
            try
            {
                FormAnuncio formAnuncio = new FormAnuncio();
                HtmlDocument doc = new HtmlDocument();

                // Load the html from a string
                doc.LoadHtml(htmlAnuncio);

                var tmp = doc.DocumentNode.SelectSingleNode("//*[@name='price']");

                int price;
                _ = int.TryParse(tmp.Attributes["value"].Value, out price);
                formAnuncio.variables.price = price;

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='title']");
                formAnuncio.variables.title = tmp.Attributes["value"].Value;

                tmp = doc.DocumentNode.SelectSingleNode("//textarea[@name='description']");
                formAnuncio.variables.description = tmp.InnerText;

                formAnuncio.variables.images = new string[0];
                List<string> imagesId = new List<string>();
                int lastPos = 0, posIni = 0, posEnd;
                posIni = htmlAnuncio.IndexOf("gcsKey", lastPos);
                while (posIni != -1)
                {
                    posEnd = htmlAnuncio.IndexOf("urls", posIni);
                    posIni += 9;
                    posEnd -= 3;
                    string id = htmlAnuncio.Substring(posIni, posEnd - posIni);
                    imagesId.Add(id);
                    lastPos = posEnd;
                    posIni = htmlAnuncio.IndexOf("gcsKey", lastPos);
                }
                formAnuncio.variables.images = imagesId.ToArray();

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='email']");
                formAnuncio.variables.email = tmp.Attributes["value"].Value;

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='name']");
                formAnuncio.variables.name = tmp.Attributes["value"].Value;

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='phone']");
                formAnuncio.variables.phone = tmp.Attributes["value"].Value;

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='subcategory']");
                formAnuncio.variables.subcategory = tmp.FirstChild.Attributes["value"].Value;

                formAnuncio.variables.contactInfo = "EMAIL_PHONE";
                formAnuncio.variables.botScore = "";

                int p1 = htmlAnuncio.IndexOf("pageProps") + "pageProps".Length + 2;
                int p2 = htmlAnuncio.IndexOf("apolloState") - 2;
                string substr = htmlAnuncio.Substring(p1, p2 - p1);
                dynamic prop = JObject.Parse(substr);
                formAnuncio.variables.token = prop.token;
                formAnuncio.variables.id = prop.id;

                return formAnuncio;
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message + "\n" + ex.StackTrace, "");
            }
        }

        private void GetException(string answer, string _uri, bool ff, CaptchaAnswer captchaResponse = null)
        {
            if (answer.Contains("Error verifying reCAPTCHA"))
            {
                string ans = Captcha2Solver.set_captcha_bad(captchaResponse.AccessToken, captchaResponse.Id);
                throw new BadCaptchaException(ans, _uri);
            }
            else if (answer.Contains("Cloudflare to restrict access"))
            {
                throw new BanedException("First Attempt", _uri);
            }
            else if (answer.Contains("Attention Required! | Cloudflare"))
            {
                throw new BanedException("api.revolico ask for Captcha", _uri);
            }
            else if (answer.Contains("Has eliminado este anuncio."))
            {
                throw new BanedException("Deteccion Anuncio Eliminado", _uri);
            }
            else if (ff &&
                     !answer.Contains("\"status\":200") &&
                     !answer.Contains("\"errors\":null") &&
                     !answer.Contains("updateAdWithoutUser"))
            {
                throw new GeneralException("Non updated | " + answer, _uri);
            }
        }
    }
}
