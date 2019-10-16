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

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private const string apiRevolico = "https://api.revolico.com/graphql/";

        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        private readonly Captcha2Solver _captchaSolver;
        readonly ILogger _log;

        public AnuncioService(ILogger log)
        {
            _log = log;
            _captchaSolver = new Captcha2Solver("50db4358d2070f7bbfbc13cc814974ae");
        }

        public AnuncioService(ApplicationDbContext dbContext, ILogger<AnuncioService> log)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
            _log = log;
            _captchaSolver = new Captcha2Solver("50db4358d2070f7bbfbc13cc814974ae");
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

        public async Task Publish(string url, string Key2Captcha)
        {
            await StartProcess(url, Key2Captcha, true);
        }

        private async Task StartProcess(string _uri, string key2captcha, bool v2)
        {
            try
            {
                string htmlAnuncio = await Requests.GetAsync(_uri);
                FormAnuncio formAnuncio = ParseFormAnuncio(htmlAnuncio);

                string captchaResponse = await ResolveCaptcha(_uri, htmlAnuncio);

                formAnuncio.variables.captchaResponse = captchaResponse;
                string jsonForm = $"[{JsonConvert.SerializeObject(formAnuncio)}]";

                await Requests.PostAsync(apiRevolico, jsonForm);
            }
            catch (Exception )
            {
                return;
            }

            //NameValueCollection textValues, imagesValues, contactValues, captchaValues;
            //HttpResponseMessage responseClient;
            //HttpClient client = new HttpClient();
            //try
            //{
            //    responseClient = await client.GetAsync(_uri);

            //    if (responseClient.StatusCode == HttpStatusCode.BadGateway)
            //    {
            //        throw new Exception("Revolico no ha respondido");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //_log.LogError("Get Anuncio " + ex.ToExceptionString());
            //    return;
            //}

            //try
            //{
            //    textValues = new NameValueCollection();
            //    imagesValues = new NameValueCollection();
            //    contactValues = new NameValueCollection();
            //    captchaValues = new NameValueCollection();

            //    string responseFromServer = await responseClient.Content.ReadAsStringAsync();
            //    responseClient.Dispose();

            //    // Create a HtmlDocummet. HTML Agility Pack
            //    HtmlDocument doc = new HtmlDocument();

            //    // Load the html from a string
            //    doc.LoadHtml(responseFromServer);

            //    var tmp = doc.DocumentNode.SelectSingleNode("//*[@name='price']");
            //    textValues.Add("price", tmp.Attributes["value"].Value);

            //    tmp = doc.DocumentNode.SelectSingleNode("//*[@name='title']");
            //    textValues.Add("title", tmp.Attributes["value"].Value);

            //    tmp = doc.DocumentNode.SelectSingleNode("//*[@name='description']");
            //    textValues.Add("description", tmp.InnerText);

            //    imagesValues.Add("ad_picture_a", "");
            //    imagesValues.Add("ad_picture_b", "");
            //    imagesValues.Add("ad_picture_c", "");

            //    tmp = doc.DocumentNode.SelectSingleNode("//*[@name='email']");
            //    contactValues.Add("email", tmp.Attributes["value"].Value);

            //    tmp = doc.DocumentNode.SelectSingleNode("//*[@name='name']");
            //    contactValues.Add("name", tmp.Attributes["value"].Value);

            //    tmp = doc.DocumentNode.SelectSingleNode("//*[@name='phone']");
            //    contactValues.Add("phone", tmp.Attributes["value"].Value);

            //    int p1 = responseFromServer.IndexOf("RECAPTCHA_V2_SITE_KEY") + "RECAPTCHA_V2_SITE_KEY".Length + 4;
            //    int p2 = responseFromServer.IndexOf("RECAPTCHA_V3_SITE_KEY") - 3;

            //    string siteKey = responseFromServer.Substring(p1, p2 - p1);
            //    captchaValues.Add("captchaId", siteKey);

            //    captchaValues.Add("send_form", "Enviar");
            //    captchaValues.Add("href", "/");
            //}
            //catch (Exception ex)
            //{
            //    //_log.LogError("Rellenando Collection " + ex.ToExceptionString());
            //    return;
            //}

            //try
            //{
            //    #region Send Captcha V2
            //    string uri2Captcha = "http://2captcha.com/in.php?";

            //    var postData = "";
            //    postData += "key=" + key2captcha;
            //    postData += "&method=userrecaptcha";
            //    postData += "&googlekey=" + captchaValues["captchaId"];
            //    postData += "&pageurl=" + _uri.ToString();

            //    captchaValues["request"] = postData;

            //    var data = Encoding.ASCII.GetBytes(postData);

            //    WebRequest request = WebRequest.Create(uri2Captcha);

            //    request.Method = "POST";

            //    request.ContentType = "application/x-www-form-urlencoded";
            //    request.ContentLength = data.Length;

            //    using (var stream = request.GetRequestStream())
            //    {
            //        stream.Write(data, 0, data.Length);
            //    }

            //    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            //    StreamReader streamReader = new StreamReader(response.GetResponseStream());

            //    var responseString = streamReader.ReadToEnd();
            //    streamReader.Close();
            //    // streamReader.Dispose();

            //    response.Close();
            //    //response.Dispose();

            //    captchaValues["identification"] = responseString.Substring(3, responseString.Length - 3);
            //    #endregion

            //    #region Request Captcha Solution V2
            //    await Task.Delay(15000);
            //    string answerUrl = "http://2captcha.com/res.php?key=" + key2captcha + "&action=get&id=" + captchaValues["identification"];
            //    for (int i = 1; i < 20; i++)
            //    {
            //        responseClient = await client.GetAsync(answerUrl);
            //        responseString = await responseClient.Content.ReadAsStringAsync();
            //        responseClient.Dispose();

            //        if (responseString.Substring(0, 2) == "OK")
            //        {
            //            captchaValues["g-recaptcha-response"] = responseString.Substring(3, responseString.Length - 3);
            //            i = 500;
            //            break;
            //        }
            //        await Task.Delay(10000);
            //    }

            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    //_log.LogError("Solving Captcha " + ex.ToExceptionString());
            //    return;
            //}

            //for (int i = 1; i < 20; i++)
            //{
            //    responseClient = await client.GetAsync(answerUrl);
            //    responseString = await responseClient.Content.ReadAsStringAsync();
            //    responseClient.Dispose();

            //    if (responseString.Substring(0, 2) == "OK")
            //    {
            //        captchaValues["g-recaptcha-response"] = responseString.Substring(3, responseString.Length - 3);
            //        i = 500;
            //        break;
            //    }
            //    await Task.Delay(10000);
            //}
            //try
            //{
            //    // Create a WebRequest for the URI
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);

            //    // If required by the server, set the credentials
            //    request.Credentials = CredentialCache.DefaultCredentials;

            //    // Codificacion Settings
            //    Encoding iso = Encoding.GetEncoding("iso-8859-1");
            //    Encoding utf8 = Encoding.UTF8;
            //    Encoding utf32 = Encoding.UTF32;

            //    // Building Boundary
            //    //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            //    string boundary = "-----------------------------265001916915724";
            //    byte[] boundarybytes = System.Text.UTF8Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            //    boundarybytes = Encoding.Convert(utf8, iso, boundarybytes);

            //    // Set Content Type
            //    request.ContentType = "multipart/form-data;boundary=" + boundary;

            //    // Set Method
            //    request.Method = "POST";

            //    request.KeepAlive = true;
            //    request.UserAgent = ".NET Framework Client";

            //    // Stream to Write data to Send
            //    using (Stream dataStreamRequest = await request.GetRequestStreamAsync())
            //    {
            //        // Contenido Anuncio
            //        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            //        foreach (string key in textValues.Keys)
            //        {
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            string formItem = string.Format(formdataTemplate, key, textValues[key]);
            //            byte[] formItemBytes = System.Text.Encoding.UTF32.GetBytes(formItem);
            //            formItemBytes = Encoding.Convert(utf32, iso, formItemBytes);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;
            //        }

            //        // Images de anuncio. Por default Vacio
            //        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            //        {
            //            string formItem, formImage;
            //            byte[] formItemBytes, formImageBytes;
            //            // Image A. Max file size.
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
            //            formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;

            //            // Image A. File
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formImage = string.Format(headerTemplate, "ad_picture_a", "", "application/octet-stream");
            //            formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
            //            dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
            //            request.ContentLength += formImageBytes.Length;

            //            // Image B. Max file size.
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
            //            formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;

            //            // Image B. File
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formImage = string.Format(headerTemplate, "ad_picture_b", "", "application/octet-stream");
            //            formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
            //            dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
            //            request.ContentLength += formImageBytes.Length;

            //            // Image C. Max file size.
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
            //            formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;

            //            // Image C. File
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            formImage = string.Format(headerTemplate, "ad_picture_c", "", "application/octet-stream");
            //            formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
            //            dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
            //            request.ContentLength += formImageBytes.Length;
            //        }


            //        // Informacion de Contacto
            //        foreach (string key in contactValues.Keys)
            //        {
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            string formItem = string.Format(formdataTemplate, key, contactValues[key]);
            //            byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;
            //        }

            //        // Informacion Captcha
            //        foreach (string key in captchaValues.Keys)
            //        {
            //            dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //            request.ContentLength += boundarybytes.Length;
            //            string formItem = string.Format(formdataTemplate, key, captchaValues[key]);
            //            byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
            //            dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
            //            request.ContentLength += formItemBytes.Length;
            //        }

            //        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
            //        request.ContentLength += boundarybytes.Length;
            //    }

            //    request.AllowAutoRedirect = false;

            //    // Obtener respuesta de solicitud
            //    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();


            //    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Found)
            //    {
            //        response.Close();
            //        response.Dispose();
            //        //estado = AnuncioEstado.Ok;
            //        return;
            //    }

            //    if (response.StatusCode == HttpStatusCode.BadGateway)
            //    {
            //        response.Close();
            //        response.Dispose();
            //        //estado = AnuncioEstado.Revolico;
            //        return;
            //    }

            //    response.Close();
            //    //response.Dispose();
            //    //estado = AnuncioEstado.CaptchaError;
            //    return;
            //}
            //catch (Exception ex)
            //{
            //    //_log.LogError("Actualizando Anuncio " + ex.ToExceptionString());
            //    return;
            //}
        }

        private async Task<string> ResolveCaptcha(string _uri, string htmlAnuncio)
        {
            int p1 = htmlAnuncio.IndexOf("RECAPTCHA_V2_SITE_KEY") + "RECAPTCHA_V2_SITE_KEY".Length + 3;
            int p2 = htmlAnuncio.IndexOf("RECAPTCHA_V3_SITE_KEY") - 3;

            string siteKey = htmlAnuncio.Substring(p1, p2 - p1);
            //string siteKey = "6LfyRCIUAAAAAP5zhuXfbwh63Sx4zqfPmh3Jnjy7";
            Dictionary<string, string> opts = new Dictionary<string, string>();
            string captchaId = _captchaSolver.submit_recaptcha(_uri, siteKey);

            await Task.Delay(15000);
            for(int i = 0; i < 20; i++)
            {
                string ans = _captchaSolver.retrieve(captchaId);
                if (!String.IsNullOrEmpty(ans))
                {
                    return ans;
                }
                await Task.Delay(10000);
            }
            throw new Exception("no resuelto");
        }

        private FormAnuncio ParseFormAnuncio(string htmlAnuncio)
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

            tmp = doc.DocumentNode.SelectSingleNode("//*textarea[@name='description']");
            formAnuncio.variables.description = tmp.InnerText;

            formAnuncio.variables.images = new string[0];
            //imagesValues.Add("ad_picture_a", "");
            //imagesValues.Add("ad_picture_b", "");
            //imagesValues.Add("ad_picture_c", "");

            tmp = doc.DocumentNode.SelectSingleNode("//*[@name='email']");
            formAnuncio.variables.email = tmp.Attributes["value"].Value;

            tmp = doc.DocumentNode.SelectSingleNode("//*[@name='name']");
            formAnuncio.variables.name = tmp.Attributes["value"].Value;

            tmp = doc.DocumentNode.SelectSingleNode("//*[@name='phone']");
            formAnuncio.variables.phone = tmp.Attributes["value"].Value;

            tmp = doc.DocumentNode.SelectSingleNode("//*[@name='subcategory']");
            formAnuncio.variables.subcategory = tmp.FirstChild.Attributes["value"].Value;
            //formAnuncio.variables.subcategory = "101";

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

    }
}
