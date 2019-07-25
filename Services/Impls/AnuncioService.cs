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

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        public AnuncioService()
        {

        }

        public AnuncioService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
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
            await repositoryAnuncio.DeleteAsync(anuncio);
        }

        public async Task Publish(string url, string Key2Captcha)
        {
            await Task.Delay(5000);
            await StartProcess(url, Key2Captcha, true);
        }

        private async Task StartProcess(string _uri, string key2captcha, bool v2)
        {
            NameValueCollection textValues, imagesValues, contactValues, captchaValues;
            HttpResponseMessage responseClient;
            HttpClient client = new HttpClient();
            try
            {
                responseClient = await client.GetAsync(_uri);

                if (responseClient.StatusCode == HttpStatusCode.BadGateway)
                {
                    throw new Exception("Revolico no ha respondido");
                }
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                textValues = new NameValueCollection();
                imagesValues = new NameValueCollection();
                contactValues = new NameValueCollection();
                captchaValues = new NameValueCollection();

                string responseFromServer = await responseClient.Content.ReadAsStringAsync();
                responseClient.Dispose();

                // Create a HtmlDocummet. HTML Agility Pack
                HtmlDocument doc = new HtmlDocument();

                // Load the html from a string
                doc.LoadHtml(responseFromServer);

                var tmp = doc.DocumentNode.SelectSingleNode("//*[@name='ad_price']");
                textValues.Add("ad_price", tmp.Attributes["value"].Value);

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='ad_headline']");
                textValues.Add("ad_headline", tmp.Attributes["value"].Value);

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='ad_text']");
                textValues.Add("ad_text", tmp.InnerText);

                imagesValues.Add("ad_picture_a", "");
                imagesValues.Add("ad_picture_b", "");
                imagesValues.Add("ad_picture_c", "");

                tmp = doc.DocumentNode.SelectSingleNode("//*[@id='email-enabled-true']");
                contactValues.Add("email_enabled", tmp.Attributes["value"].Value);

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='name']");
                contactValues.Add("name", tmp.Attributes["value"].Value);

                tmp = doc.DocumentNode.SelectSingleNode("//*[@name='phone']");
                contactValues.Add("phone", tmp.Attributes["value"].Value);


                tmp = doc.DocumentNode.SelectSingleNode("//*[@class='g-recaptcha']");
                captchaValues.Add("captchaId", tmp.Attributes["data-sitekey"].Value);

                captchaValues.Add("send_form", "Enviar");
                captchaValues.Add("href", "/");
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                #region Send Captcha V2
                string uri2Captcha = "http://2captcha.com/in.php?";

                var postData = "";
                postData += "key=" + key2captcha;
                postData += "&method=userrecaptcha";
                postData += "&googlekey=" + captchaValues["captchaId"];
                postData += "&pageurl=" + _uri.ToString();

                captchaValues["request"] = postData;

                var data = Encoding.ASCII.GetBytes(postData);

                WebRequest request = WebRequest.Create(uri2Captcha);

                request.Method = "POST";

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                var responseString = streamReader.ReadToEnd();
                streamReader.Close();
                // streamReader.Dispose();

                response.Close();
                //response.Dispose();

                captchaValues["identification"] = responseString.Substring(3, responseString.Length - 3);
                #endregion

                #region Request Captcha Solution V2
                await Task.Delay(15000);
                string answerUrl = "http://2captcha.com/res.php?key=" + key2captcha + "&action=get&id=" + captchaValues["identification"];
                for (int i = 1; i < 8; i++)
                {
                    responseClient = await client.GetAsync(answerUrl);
                    responseString = await responseClient.Content.ReadAsStringAsync();
                    responseClient.Dispose();

                    if (responseString.Substring(0, 2) == "OK")
                    {
                        captchaValues["g-recaptcha-response"] = responseString.Substring(3, responseString.Length - 3);
                        i = 12;
                        break;
                    }
                    await Task.Delay(15000);
                }

                #endregion
            }
            catch (Exception )
            {
                return;
            }

            try
            {
                // Create a WebRequest for the URI
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_uri);

                // If required by the server, set the credentials
                request.Credentials = CredentialCache.DefaultCredentials;

                // Codificacion Settings
                Encoding iso = Encoding.GetEncoding("iso-8859-1");
                Encoding utf8 = Encoding.UTF8;
                Encoding utf32 = Encoding.UTF32;

                // Building Boundary
                //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                string boundary = "-----------------------------265001916915724";
                byte[] boundarybytes = System.Text.UTF8Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                boundarybytes = Encoding.Convert(utf8, iso, boundarybytes);

                // Set Content Type
                request.ContentType = "multipart/form-data;boundary=" + boundary;

                // Set Method
                request.Method = "POST";

                request.KeepAlive = true;
                request.UserAgent = ".NET Framework Client";

                // Stream to Write data to Send
                using (Stream dataStreamRequest = await request.GetRequestStreamAsync())
                {
                    // Contenido Anuncio
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in textValues.Keys)
                    {
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        string formItem = string.Format(formdataTemplate, key, textValues[key]);
                        byte[] formItemBytes = System.Text.Encoding.UTF32.GetBytes(formItem);
                        formItemBytes = Encoding.Convert(utf32, iso, formItemBytes);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;
                    }

                    // Images de anuncio. Por default Vacio
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    {
                        string formItem, formImage;
                        byte[] formItemBytes, formImageBytes;
                        // Image A. Max file size.
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                        formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;

                        // Image A. File
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formImage = string.Format(headerTemplate, "ad_picture_a", "", "application/octet-stream");
                        formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                        dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
                        request.ContentLength += formImageBytes.Length;

                        // Image B. Max file size.
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                        formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;

                        // Image B. File
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formImage = string.Format(headerTemplate, "ad_picture_b", "", "application/octet-stream");
                        formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                        dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
                        request.ContentLength += formImageBytes.Length;

                        // Image C. Max file size.
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                        formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;

                        // Image C. File
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        formImage = string.Format(headerTemplate, "ad_picture_c", "", "application/octet-stream");
                        formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                        dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
                        request.ContentLength += formImageBytes.Length;
                    }


                    // Informacion de Contacto
                    foreach (string key in contactValues.Keys)
                    {
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        string formItem = string.Format(formdataTemplate, key, contactValues[key]);
                        byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;
                    }

                    // Informacion Captcha
                    foreach (string key in captchaValues.Keys)
                    {
                        dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                        request.ContentLength += boundarybytes.Length;
                        string formItem = string.Format(formdataTemplate, key, captchaValues[key]);
                        byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                        dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                        request.ContentLength += formItemBytes.Length;
                    }

                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    request.ContentLength += boundarybytes.Length;
                }
                
                request.AllowAutoRedirect = false;
                
                // Obtener respuesta de solicitud
                HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
                

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Found)
                {
                    response.Close();
                    response.Dispose();
                    //estado = AnuncioEstado.Ok;
                    return;
                }

                if (response.StatusCode == HttpStatusCode.BadGateway)
                {
                    response.Close();
                    response.Dispose();
                    //estado = AnuncioEstado.Revolico;
                    return;
                }

                response.Close();
                //response.Dispose();
                //estado = AnuncioEstado.CaptchaError;
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
