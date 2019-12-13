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
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        private readonly INotificationsService _notificationsService;
        private readonly ITemporizadorService _temporizadorService;
        readonly ILogger _log;

        public AnuncioService(ApplicationDbContext dbContext, ILogger<AnuncioService> log, INotificationsService notificationsService, ITemporizadorService temporizadorService)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
            _log = log;
            _notificationsService = notificationsService;
            _temporizadorService = temporizadorService;
            initCat();
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
            if (await _temporizadorService.GroupHasTemporizadoresEnable(GrupoId))
            {
                throw new CannotDeleteException("El Grupo tiene Temporizadores habilitados");
            }
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.FindAllAsync(p => p.GroupId == GrupoId);
            if(anuncios.Any(a => a.Procesando != 0))
            {
                throw new CannotDeleteException("Existen anuncios siendo procesados en el sistema");
            }
            repositoryAnuncio.RemoveRange(anuncios);

            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task<IEnumerable<Anuncio>> GetAnunciosToUpdate(string GroupId, int Etapa)
        {
            IEnumerable<Anuncio> listAnuncio = new List<Anuncio>();
            if (Etapa == 0)
            {
                listAnuncio = await QueryBaseAnunciosToUpdate(GroupId).ToListAsync();
            }
            else if (Etapa > 0)
            {
                listAnuncio = await QueryBaseAnunciosToUpdate(GroupId).Where(a => !a.Actualizado).Take(Etapa).ToListAsync();

                if (!listAnuncio.Any())
                {
                    listAnuncio = await QueryBaseAnunciosToUpdate(GroupId).ToListAsync();
                    foreach (Anuncio a in listAnuncio)
                    {
                        a.Actualizado = false;
                    }
                    listAnuncio = listAnuncio.Take(Etapa);
                }
            }
            else
            {
                return listAnuncio;
            }
            
            foreach(Anuncio a in listAnuncio)
            {
                a.Procesando = 1;
                a.Actualizado = true;
            }

            return listAnuncio;
        }

        private IQueryable<Anuncio> QueryBaseAnunciosToUpdate(string GroupId)
        {
            return repositoryAnuncio.QueryAll().Where(a => a.GroupId == GroupId
                                                        && a.Enable.HasValue && a.Enable.Value
                                                        && a.Procesando == 0
                                                        && !a.Eliminado 
                                                        && !a.Despublicado)
                                               .OrderBy(a => a.Orden)
                                               .Select(a => a);
        }

        public async Task DeleteAsync(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(p => p.Id == Id);
            if(anuncio.Procesando != 0)
            {
                throw new CannotDeleteException("El anuncio está siendo procesado por el sistema");
            }
            if (anuncio.Enable == true && await _temporizadorService.GroupHasTemporizadoresEnable(anuncio.GroupId))
            {
                throw new CannotDeleteException("El Grupo tiene Temporizadores activados y su anuncio está activo");
            }
            repositoryAnuncio.Remove(anuncio);
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task NotifyDelete(List<Anuncio> list)
        {
            try
            {
                if (!list.Any())
                {
                    return;
                }

                List<Notificacion> notificacions = new List<Notificacion>();
                foreach (Anuncio item in list)
                {
                    _dbContext.Entry(item).Reference(s => s.Grupo).Load();
                    _log.LogInformation("Anuncio caducado/eliminado/despublicado " + item.Id + " -> " + item.Url);
                    notificacions.Add(new Notificacion()
                    {
                        UserId = item.Grupo.UserId,
                        DateCreated = DateTime.Now.ToUtcCuba(),
                        Mensaje = String.Format("Del grupo {0} el anuncio {1} a caducado/eliminado/despublicado por tanto se ha deshabilitado en el sistema.\nUrl {2}\nCategoría: {3}", item.Grupo.Nombre, item.Titulo, item.Url, item.Categoria),
                        Readed = false
                    });
                }

                await _notificationsService.Add(notificacions);
            }
            catch(Exception ex)
            {
                _log.LogError(ex.ToExceptionString());
            }
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
                anuncio.FormUpdateAnuncio = JsonConvert.SerializeObject(formAnuncio);

                //Solve Captcha
                captchaResponse = await ResolveCaptcha(Key2Captcha, Requests.RevolicoInserrUrl, htmlAnuncio);
                formAnuncio.variables.captchaResponse = captchaResponse.Answerv2;
                //formAnuncio.variables.botScore = captchaResponse.Answerv3;

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
            catch (Exception ex)
            {
                result = new ReinsertResult(anuncio, ex);
            }

            return result;
        }

        public async Task<string> InsertAnuncio(FormInsertAnuncio formInsertAnuncio)
        {
            string jsonForm = $"[{JsonConvert.SerializeObject(formInsertAnuncio)}]";

            return await Requests.PostAsync(Requests.apiRevolico, jsonForm);
        }

        public InsertResult ParseInsertResult(string answer)
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
            catch (Exception)
            {
                Url = $"{Requests.RevolicoModifyUrl}?key={formDeleteAnuncio.variables.token}{formDeleteAnuncio.variables.id}";
                throw new Exception("Error Removing from Revolico " + Url);
            }

            Url = $"{Requests.RevolicoModifyUrl}?key={formDeleteAnuncio.variables.token}{formDeleteAnuncio.variables.id}";
            throw new Exception("Error Removing from Revolico " + Url);
        }

        public async Task<CaptchaAnswer> ResolveCaptcha(string key2captcha, string _uri, string htmlAnuncio)
        {
            int p1 = htmlAnuncio.IndexOf("RECAPTCHA_V2_SITE_KEY") + "RECAPTCHA_V2_SITE_KEY".Length + 3;
            int p2 = htmlAnuncio.IndexOf("RECAPTCHA_V3_SITE_KEY") - 3;

            string siteKeyv2 = htmlAnuncio.Substring(p1, p2 - p1);
            string siteKeyv3 = "6Lfw9oYUAAAAAIjIAhcI2lpRHp5IfrmJv-asUrvp";
            //string siteKey = "6LfyRCIUAAAAAP5zhuXfbwh63Sx4zqfPmh3Jnjy7";
            string captchaIdv2 = await Captcha2Solver.submit_recaptcha(key2captcha, _uri, siteKeyv2, false);
            //string captchaIdv3 = await Captcha2Solver.submit_recaptcha(key2captcha, _uri, siteKeyv3, true);
            string captchaIdv3 = "";
            WebException last = null;

            string finalAnsv2 = string.Empty, finalAnsv3 = string.Empty;
            await Task.Delay(15000);
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(finalAnsv2))
                    {
                        string ansv2 = await Captcha2Solver.retrieve(key2captcha, captchaIdv2);
                        if (!String.IsNullOrEmpty(ansv2))
                        {
                            finalAnsv2 = ansv2;
                            return new CaptchaAnswer(key2captcha, captchaIdv2, captchaIdv3, finalAnsv2, finalAnsv3);
                        }
                    }
                    //if (string.IsNullOrEmpty(finalAnsv3))
                    //{
                    //    string ansv3 = await Captcha2Solver.retrieve(key2captcha, captchaIdv3);
                    //    if (!String.IsNullOrEmpty(ansv3))
                    //    {
                    //        finalAnsv3 = ansv3;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(finalAnsv3) && !string.IsNullOrEmpty(finalAnsv2))
                    //{
                    //    return new CaptchaAnswer(key2captcha, captchaIdv2, captchaIdv3, finalAnsv2, finalAnsv3);
                    //}

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
            throw new Exception("Unkown error. Captcha");
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
                throw new GeneralException(ex.Message + "\n" + ex.StackTrace + "\n --- \n" + htmlAnuncio, "");
            }
        }

        private void GetException(string answer, string _uri, bool ff, CaptchaAnswer captchaResponse = null)
        {
            if (answer.Contains("Error verifying reCAPTCHA"))
            {
                //string ans = Captcha2Solver.set_captcha_bad(captchaResponse.AccessToken, captchaResponse.Id);
                //throw new BaseException("Bad Captcha", "Bad Captcha " + ans);
                throw new BaseException("Bad Captcha", "Bad Captcha");
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
            else if (answer.ToLower().Contains("anuncio despublicado"))
            {
                throw new BaseException(string.Empty, "Deteccion Anuncio Despublicado");
            }
            else if (answer.Contains("Ha ocurrido un error"))
            {
                throw new BaseException(string.Empty, "Revolico Error");
            }
            else if (ff && (
                     !answer.Contains("\"status\":200") ||
                     !answer.Contains("\"errors\":null") ||
                     !answer.Contains("createAdWithoutUser")))
            {
                throw new BaseException(string.Empty, "Non updated " + answer);
            }
            else if (answer.Contains("Anuncio despublicado."))
            {
                throw new BaseException(string.Empty, "Deteccion Anuncio despublicado");
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
            foreach (Anuncio a in anunciosProcesados)
            {
                await repositoryAnuncio.UpdateAsync(a, a.Id);
            }
            await repositoryAnuncio.SaveChangesAsync();
        }

        public async Task<FormInsertAnuncio> Retrieve(string url)
        {
            // Get Anuncio
            string htmlAnuncio = await Requests.GetAsync(url);
            FormInsertAnuncio formInsert = ParseFormReadUrl(htmlAnuncio);
            return formInsert;
        }

        public FormInsertAnuncio ParseFormReadUrl(string htmlAnuncio)
        {
            try
            {
                FormInsertAnuncio formAnuncio = new FormInsertAnuncio();
                HtmlDocument doc = new HtmlDocument();

                // Load the html from a string
                doc.LoadHtml(htmlAnuncio);
                HtmlNode tmp;
                try
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adPrice']");

                    int price;
                    _ = int.TryParse(tmp.InnerText, out price);
                    formAnuncio.variables.price = price;
                }
                catch (Exception) { }

                tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adTitle']");
                formAnuncio.variables.title = HttpUtility.HtmlDecode(tmp.InnerText);

                tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adDescription']");
                int p1 = htmlAnuncio.LastIndexOf("description\":\"") + "description\":\"".Length;
                int p2 = htmlAnuncio.IndexOf("\",\"price", p1);
                // Decode the encoded string.
                formAnuncio.variables.description = HttpUtility.HtmlDecode(htmlAnuncio).Substring(p1, p2 - p1);

                formAnuncio.variables.images = new string[0];
                List<string> imagesId = new List<string>();
                int lastPos = 0, posIni = 0, posEnd;
                posIni = htmlAnuncio.IndexOf("gcsKey", lastPos);
                while (posIni != -1)
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

                try
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adEmail']");
                    formAnuncio.variables.email = tmp.InnerText;
                }
                catch (Exception) { }

                try
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adName']");
                    formAnuncio.variables.name = tmp.InnerText;
                }
                catch (Exception) { }

                try
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@data-cy='adPhone']");
                    formAnuncio.variables.phone = tmp.InnerText;
                }
                catch (Exception) { }

                //int p2 = htmlAnuncio.LastIndexOf("\",\"typename\":\"CategoryType\"");
                //int p1 = p2;
                //for (int i = 1; i <= 3; i++)
                //{
                //    if (htmlAnuncio.ElementAt(p2 - i) == ':')
                //    {
                //        p1 = p2 - i + 1;
                //        break;
                //    }
                //}
                string cat = GetCategoria(htmlAnuncio);
                formAnuncio.variables.subcategory = Cat[cat];

                formAnuncio.variables.contactInfo = "EMAIL_PHONE";
                formAnuncio.variables.botScore = "";

                Console.WriteLine(cat);

                return formAnuncio;
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message + "\n" + ex.StackTrace, "");
            }
        }

        NameValueCollection Cat;
        void initCat()
        {
            Cat = new NameValueCollection();
            Cat.Add("/vivienda/alquiler-a-cubanos/", "103");
            Cat.Add("/vivienda/alquiler-a-extranjeros/", "104");
            Cat.Add("/servicios/gimnasio-masaje-entrenador/", "220");
            Cat.Add("/vivienda/compra-venta/", "101");
            Cat.Add("/vivienda/casa-en-la-playa/", "105");
            Cat.Add("/compra-venta/electrodomesticos/", "35");
            Cat.Add("/servicios/construccion-mantenimiento/", "75");
            Cat.Add("/servicios/diseno-decoracion/", "79");
            Cat.Add("/compra-venta/libros-revistas/", "38");
            Cat.Add("/compra-venta/consola-videojuego-juegos/", "39");
            Cat.Add("/compra-venta/mascotas-animales/", "41");
            Cat.Add("/computadoras/impresora-cartuchos/", "15");
            Cat.Add("/compra-venta/ropa-zapato-accesorios/", "211");
            Cat.Add("/compra-venta/joyas-relojes/", "211");
            Cat.Add("/autos/piezas-accesorios/", "125");
        }

        public async Task Reset()
        {
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.GetAllAsync();
            foreach (Anuncio item in anuncios){
                item.Procesando = 0;
                item.Revalidado = 0;
            }
        }

        public async Task TogleAnuncio(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(a => a.Id == Id);
            if(anuncio.Enable == true)
            {
                anuncio.Enable = false;
            }
            else
            {
                anuncio.Enable = true;
            }
            await repositoryAnuncio.SaveChangesAsync();
        }
    }
}
