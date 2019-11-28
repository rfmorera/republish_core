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
using System.Web;
using Services.Results;

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private const string noiseData = "\n\n\n\n\n\n\n\n\n-----------Raw Text-------------------\nqwertyuiopas nbghnhfntgy,lop kjhmgymikonbvfvbcyh\n xcvxb ztfdwqerasfvtyrfjguioyhio pujdfghjklzxcvbm\nzqxswcedvfrb tgnhymju,ik.lo\n123456789-+.0\n??|?|?|?| ?|?||?||?|? ?|?|?|?|?|?|?| ?|?||?|?|?\n___________________ ____________________ ______________\n//////////////////////////// ///////////////// /////////////// ///////////\n";

        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        private readonly INotificationsService _notificationsService;
        readonly ILogger _log;

        public AnuncioService(ApplicationDbContext dbContext, ILogger<AnuncioService> log, INotificationsService notificationsService)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
            _log = log;
            _notificationsService = notificationsService;
        }

        public async Task AddAsync(string GrupoId, string[] links)
        {
            int len = links.Length;
            for (int i = 0; i < len; i++)
            {
                try
                {
                    Anuncio anuncio = new Anuncio() { UrlFormat = new Uri(links[i]), GroupId = GrupoId };
                    repositoryAnuncio.Add(anuncio);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.ToExceptionString());
                }
            }
            await repositoryAnuncio.SaveChangesAsync();
            await UpdateTitle(GrupoId);
        }

        public async Task UpdateTitle(string GrupoId)
        {
            IEnumerable<Anuncio> anuncios = await GetByGroup(GrupoId);
            IEnumerable<FormUpdateAnuncio> dataAnuncios = GetData(anuncios.Select(a => a.Url).ToArray());

            int len = anuncios.Count();
            for (int i = 0; i < len; i++)
            {
                try
                {
                    FormUpdateAnuncio d = dataAnuncios.ElementAt(i);
                    string t = d.variables.title, c = d.variables.categoria;
                    if (String.IsNullOrEmpty(t) && String.IsNullOrEmpty(anuncios.ElementAt(i).Titulo))
                    {
                        anuncios.ElementAt(i).Titulo = "-- título no actualizado -- ";
                    }
                    else if (!String.IsNullOrEmpty(t))
                    {
                        anuncios.ElementAt(i).Titulo = t;
                    }
                    if (!String.IsNullOrEmpty(c))
                    {
                        anuncios.ElementAt(i).Categoria = c;
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.ToExceptionString());
                }
            }

            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task<IEnumerable<Anuncio>> GetByGroup(string GrupoId)
        {
            return (await repositoryAnuncio.FindAllAsync(a => a.GroupId == GrupoId)).AsEnumerable();
        }

        private IEnumerable<FormUpdateAnuncio> GetData(string[] links)
        {
            List<Task<string>> body = new List<Task<string>>();
            foreach (string st in links)
            {
                body.Add(Requests.GetAsync(st));
            }

            try
            {
                Task.WaitAll(body.ToArray());
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToExceptionString());
            }

            List<FormUpdateAnuncio> ans = new List<FormUpdateAnuncio>();

            int len = links.Length;
            for (int i = 0; i < len; i++)
            {
                try
                {
                    FormUpdateAnuncio formAnuncio = ParseFormAnuncio(body[i].Result);
                    ans.Add(formAnuncio);
                }
                catch (Exception) { ans.Add(null); }
            }

            return ans;
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
            if (!list.Any())
            {
                return;
            }

            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.QueryAll()
                                                                    .Where(a => list.Contains(a.Id))
                                                                    .Include(a => a.Grupo)
                                                                    .ToListAsync();
            List<Notificacion> notificacions = new List<Notificacion>();
            foreach (Anuncio item in anuncios)
            {
                notificacions.Add(new Notificacion()
                {
                    UserId = item.Grupo.UserId,
                    DateCreated = DateTime.Now.ToUtcCuba(),
                    Mensaje = String.Format("Del grupo {0} el anuncio {1} a caducado/eliminado por tanto se ha eliminado del sistema.\nUrl {2}\nCategoría: {3}", item.Grupo.Nombre, item.Titulo, item.Url, item.Categoria),
                    Readed = false
                });
            }

            await _notificationsService.Add(notificacions);

            await DeleteAsync(anuncios);
        }

        public async Task<ReinsertResult> ReInsert(Anuncio anuncio, string Key2Captcha, string email)
        {
            ReinsertResult result;
            CaptchaAnswer captchaResponse;
            FormInsertAnuncio formInsertAnuncio;
            FormDeleteAnuncio formDeleteAnuncio;
            try
            {
                // Get Anuncio
                string htmlAnuncio = await Requests.GetAsync(anuncio.Url);
                GetException(htmlAnuncio, anuncio.Url, false);

                // Parse All Data
                FormUpdateAnuncio formAnuncio = ParseFormAnuncio(htmlAnuncio);
                formAnuncio.variables.email = email;

                //Solve Captcha
                captchaResponse = await ResolveCaptcha(Key2Captcha, Requests.RevolicoInserrUrl, htmlAnuncio);
                formAnuncio.variables.captchaResponse = captchaResponse.Answer;

                // Parse Insert and Delete Forms
                formInsertAnuncio = new FormInsertAnuncio(formAnuncio);
                formDeleteAnuncio = new FormDeleteAnuncio(formAnuncio);

                // Insert new Announce
                string answer = await InsertAnuncio(formInsertAnuncio);

                // Verify Insertion
                GetException(answer, anuncio.Url, true, captchaResponse);

                // Update new Anuncio URL
                InsertResult insertResult = ParseInsertResult(answer);
                anuncio.Url = $"{Requests.RevolicoModifyUrl}?key={insertResult.FullId}";
                _log.LogWarning($"ReplaceInsert {anuncio.Id} {insertResult.FullId}");

                // Delete from Revolico
                await DeleteFromRevolico(formDeleteAnuncio);

                result = new ReinsertResult(anuncio);
            }
            catch(Exception ex)
            {
                result = new ReinsertResult(anuncio, ex);
            }

            return result;
        }

        private async Task<string> InsertAnuncio(FormInsertAnuncio formInsertAnuncio)
        {
            string jsonForm = $"[{JsonConvert.SerializeObject(formInsertAnuncio)}]";

            return await Requests.PostAsync(Requests.apiRevolico, jsonForm);
        }

        private InsertResult ParseInsertResult(string answer)
        {
            int posIni = answer.IndexOf("id\":\"") + "id\":\"".Length;
            int posNex = answer.IndexOf("\"", posIni);
            string Id = answer.Substring(posIni, posNex - posIni);

            posIni = answer.IndexOf("token\":\"") + "token\":\"".Length;
            posNex = answer.IndexOf("\"", posIni);
            string Token = answer.Substring(posIni, posNex - posIni);
            return new InsertResult(Id, Token);
        }

        public async Task<bool> DeleteFromRevolico(string url)
        {
            string htmlAnuncio = await Requests.GetAsync(url);
            FormUpdateAnuncio formAnuncio = ParseFormAnuncio(htmlAnuncio);

            FormDeleteAnuncio formDeleteAnuncio = new FormDeleteAnuncio(formAnuncio);

            return await DeleteFromRevolico(formDeleteAnuncio);
        }

        public async Task<bool> DeleteFromRevolico(FormDeleteAnuncio formDeleteAnuncio)
        {
            string Url;
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    string jsonForm = $"[{JsonConvert.SerializeObject(formDeleteAnuncio)}]";
                    string answer = await Requests.PostAsync(Requests.apiRevolico, jsonForm);
                    if (answer.Contains("\"status\":200") ||
                         answer.Contains("\"errors\":null") ||
                         answer.Contains("DeleteAdWithoutUserMutationPayload"))
                    {
                        return true;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(20));
                }
            }
            catch(Exception)
            {
                Url = $"{Requests.RevolicoModifyUrl}?key={formDeleteAnuncio.variables.token}{formDeleteAnuncio.variables.id}";
                throw new Exception("Error Removing from Revolico " + Url);
            }

            Url = $"{Requests.RevolicoModifyUrl}?key={formDeleteAnuncio.variables.token}{formDeleteAnuncio.variables.id}";
            throw new Exception("Error Removing from Revolico " + Url);
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

        public FormUpdateAnuncio ParseFormAnuncio(string htmlAnuncio)
        {
            try
            {
                FormUpdateAnuncio formAnuncio = new FormUpdateAnuncio();
                HtmlDocument doc = new HtmlDocument();

                // Load the html from a string
                doc.LoadHtml(htmlAnuncio);

                var tmp = doc.DocumentNode.SelectSingleNode("//*[@name='price']");

                int price;
                _ = int.TryParse(tmp.Attributes["value"].Value, out price);
                formAnuncio.variables.price = price;
                
                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='title']");
                formAnuncio.variables.title = HttpUtility.HtmlDecode(tmp.Attributes["value"].Value);

                tmp = doc.DocumentNode.SelectSingleNode("//textarea[@name='description']");
                // Decode the encoded string.
                formAnuncio.variables.description = HttpUtility.HtmlDecode(tmp.InnerText);

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

                formAnuncio.variables.categoria = GetCategoria(htmlAnuncio);

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
                throw new BaseException("Bad Captcha", "Bad Captcha " + ans);
            }
            else if (answer.Contains("Cloudflare to restrict access"))
            {
                throw new BaseException(string.Empty, "Baned CloudFlare");
            }
            else if (answer.Contains("Attention Required! | Cloudflare"))
            {
                throw new BaseException(string.Empty, "api.revolico ask for Captcha");
            }
            else if (answer.Contains("Has eliminado este anuncio."))
            {
                throw new BaseException(string.Empty, "Deteccion Anuncio Eliminado");
            }
            else if (ff && (
                     !answer.Contains("\"status\":200") ||
                     !answer.Contains("\"errors\":null") ||
                     !answer.Contains("createAdWithoutUser")))
            {
                throw new BaseException(string.Empty, "Non updated " + answer);
            }
        }

        private string GetCategoria(string content)
        {
            int posIni = content.IndexOf("breadcrumb"), cnt = 2, posEnd;
            while (cnt > 0)
            {
                posIni = content.IndexOf("<a href", posIni + 1);
                cnt--;
            }
            posIni += 9;
            posEnd = content.IndexOf("\"", posIni);
            return content.Substring(posIni, posEnd - posIni);
        }

        public async Task Update(List<Anuncio> anunciosProcesados)
        {
            foreach(Anuncio a in anunciosProcesados)
            {
                await repositoryAnuncio.UpdateAsync(a, a.Id);
            }
            await repositoryAnuncio.SaveChangesAsync();
        }
    }
}
