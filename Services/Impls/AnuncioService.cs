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

namespace Services.Impls
{
    public class AnuncioService : IAnuncioService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Anuncio> repositoryAnuncio;
        public AnuncioService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryAnuncio = new Repository<Anuncio>(dbContext);
        }

        public async Task AddAsync(string GrupoId, string[] links)
        {
            foreach(string st in links)
            {
                try
                {
                    Anuncio anuncio = new Anuncio() { UrlFormat = new Uri(st), GroupId = GrupoId};
                    await _dbContext.Set<Anuncio>().AddAsync(anuncio);
                }
                catch (Exception) { }
                
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllByGroup(string GrupoId)
        {
            IEnumerable<Anuncio> anuncios = await repositoryAnuncio.FindAllAsync(p => p.GroupId == GrupoId);
            foreach(Anuncio a in anuncios)
            {
                _dbContext.Set<Anuncio>().Remove(a);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id)
        {
            Anuncio anuncio = await repositoryAnuncio.FindAsync(p => p.Id == Id);
            await repositoryAnuncio.DeleteAsync(anuncio);
        }

        public void Publish(string url, string Key2Captcha)
        {
            StartProcess(url, Key2Captcha, true);
        }

        private void StartProcess(string _uri, string key2Captcha, bool v2)
        {
            try
            {
                NameValueCollection textValues, imagesValues, contactValues, captchaValues;
                //ManageLog.Log("inicio web request");
                WebResponse anuncioModificar = RequestAnuncio(_uri);
                //ManageLog.Log("inicio field anuncio");
                FieldsAnuncios(anuncioModificar, out textValues, out imagesValues, out contactValues, out captchaValues, v2);
                //ManageLog.Log("inicio solve captcha");
                if (v2)
                    SolveCaptchav2(ref captchaValues, key2Captcha, _uri);
                else
                    SolveCaptcha(ref captchaValues, key2Captcha);

                //ManageLog.Log("inicio refresh validation");
                bool refreshValidation = RequestRefresh(_uri, textValues, imagesValues, contactValues, captchaValues);
                //ManageLog.Log("fin anuncio");
            }
            catch (Exception)
            {
            }
        }

        private WebResponse RequestAnuncio(string _uri)
        {
            try
            {
                WebRequest request = WebRequest.Create(_uri);
                ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
                request.Credentials = CredentialCache.DefaultCredentials;
                //Console.WriteLine(((HttpWebRequest)request).ServicePoint.Address.ToString());
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.BadGateway)
                {
                    //estado = AnuncioEstado.Revolico;
                    throw new Exception("Revolico no ha respondido");
                }

                //var responseRequest = (HttpWebResponse)(await request.GetResponseAsync());

                return response;
            }
            catch (Exception ex)
            {
                //estado = AnuncioEstado.InternetConnection;
                throw ex;
            }
        }

        private void FieldsAnuncios(WebResponse response, out NameValueCollection textValues, out NameValueCollection imagesValues, out NameValueCollection contactValues, out NameValueCollection captchaValues, bool v2)
        {
            try
            {
                textValues = new NameValueCollection();
                imagesValues = new NameValueCollection();
                contactValues = new NameValueCollection();
                captchaValues = new NameValueCollection();

                // Get the stream containing content returned by the server
                StreamReader dataStreamResponse = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));

                // Open the stream using a StreamReader for easy access
                //StreamReader reader = new StreamReader(dataStreamResponse, Encoding.GetEncoding("iso-8859-1"));
                var responseFromServer = dataStreamResponse.ReadToEnd();
                dataStreamResponse.Close();
                //dataStreamResponse.Dispose();

                // Read the content.              

                //reader.Close();
                //reader.Dispose();

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

                if (v2 == false)
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@id='captcha']");
                    captchaValues.Add("captchaURL", tmp.Attributes["src"].Value);

                    tmp = doc.DocumentNode.SelectSingleNode("//*[@id='captchaId']");
                    captchaValues.Add("captchaId", tmp.Attributes["value"].Value);
                }
                else
                {
                    tmp = doc.DocumentNode.SelectSingleNode("//*[@class='g-recaptcha']");
                    captchaValues.Add("captchaId", tmp.Attributes["data-sitekey"].Value);
                }

                captchaValues.Add("send_form", "Enviar");
                captchaValues.Add("href", "/");


                /*
                // Get the captcha related to announcement
                var captchaImg = doc.DocumentNode.SelectSingleNode("//*[@id='captcha']");

                // Build captcha url
                string captchaUrl = "https://www.revolico.com" + captchaImg.Attributes[1].Value;
                // Show captcha URL
                Console.WriteLine(captchaUrl);*/
            }
            catch (Exception ex)
            {
                //estado = AnuncioEstado.Revolico;
                throw ex;
            }
        }

        private void SolveCaptcha(ref NameValueCollection captchaValues, string key2captcha)
        {

            try
            {

                // Create a WebRequest for the URI
                WebRequest request = (HttpWebRequest)WebRequest.Create("http://2captcha.com/in.php");

                // If required by the server, set the credentials
                request.Credentials = CredentialCache.DefaultCredentials;

                // Building Boundary
                //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                string boundary = "-----------------------------265001916915724";
                byte[] boundarybytes = System.Text.UTF8Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                // Set Content Type
                request.ContentType = "multipart/form-data;boundary=" + boundary;

                // Set Method
                request.Method = "POST";

                ((HttpWebRequest)request).KeepAlive = true;
                ((HttpWebRequest)request).UserAgent = ".NET Framework Client";

                // Stream to Write data to Send
                Stream dataStreamRequest = request.GetRequestStream();

                // Contenido Anuncio
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";


                dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                string formItem = string.Format(formdataTemplate, "key", key2captcha);
                byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);

                // Images de captcha. Header
                string captchaUrl = "https://www.revolico.com" + captchaValues["captchaUrl"];
                dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string formImage = string.Format(headerTemplate, "file", "captcha" + captchaValues["captchaId"] + ".png", "image/png");
                byte[] formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);

                // Images de captcha. Content
                byte[] imageBytes = new WebClient().DownloadData(captchaUrl);
                dataStreamRequest.Write(imageBytes, 0, imageBytes.Length);

                dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);

                captchaValues.Remove("captchaUrl");

                // Send Request
                dataStreamRequest.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //dataStreamRequest.Dispose();

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                var responseString = streamReader.ReadToEnd();
                streamReader.Close();
                //streamReader.Dispose();

                string answerUrl = "http://2captcha.com/res.php?key=" + key2captcha + "&action=get&id=" + responseString.Substring(3, responseString.Length - 3);

                Thread.Sleep(5000);
                for (int i = 1; i < 8; i++)
                {
                    //Console.WriteLine(id + " > " + "Solving {0}-th", i);
                    request = WebRequest.Create(answerUrl);
                    response = (HttpWebResponse)request.GetResponse();
                    streamReader = new StreamReader(response.GetResponseStream());

                    responseString = streamReader.ReadToEnd();

                    streamReader.Close();
                    //streamReader.Dispose();

                    response.Close();
                    //response.Dispose();

                    if (responseString.Substring(0, 2) == "OK")
                    {
                        captchaValues["captcha_code"] = responseString.Substring(3, responseString.Length - 3);
                        return;
                    }
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                //estado = AnuncioEstado.CaptchaError;
                throw e;
            }
        }

        private void SolveCaptchav2(ref NameValueCollection captchaValues, string key2Captcha, string uri)
        {
            try
            {
                #region Send Captcha V2
                string uri2Captcha = "http://2captcha.com/in.php?";

                var postData = "";
                postData += "key=" + key2Captcha;
                postData += "&method=userrecaptcha";
                postData += "&googlekey=" + captchaValues["captchaId"];
                postData += "&pageurl=" + uri.ToString();

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

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                var responseString = streamReader.ReadToEnd();
                streamReader.Close();
                // streamReader.Dispose();

                response.Close();
                //response.Dispose();

                captchaValues["identification"] = responseString.Substring(3, responseString.Length - 3);
                #endregion

                #region Request Captcha Solution V2
                Thread.Sleep(60000);
                string answerUrl = "http://2captcha.com/res.php?key=" + key2Captcha + "&action=get&id=" + captchaValues["identification"];
                for (int i = 1; i < 8; i++)
                {
                    request = WebRequest.Create(answerUrl);
                    response = (HttpWebResponse)request.GetResponse();
                    streamReader = new StreamReader(response.GetResponseStream());
                    responseString = streamReader.ReadToEnd();
                    streamReader.Close();
                    //streamReader.Dispose();

                    response.Close();
                    //response.Dispose();
                    //AnunciosException.Check2CaptchaAnswer(responseString);

                    if (responseString.Substring(0, 2) == "OK")
                    {
                        captchaValues["g-recaptcha-response"] = responseString.Substring(3, responseString.Length - 3);
                        i = 12;
                        return;
                    }
                    Thread.Sleep(20000);
                }

                #endregion
            }
            catch (Exception ex)
            {
                //estado = AnuncioEstado.CaptchaError;
                throw ex;
            }
        }

        private bool RequestRefresh(string _uri, NameValueCollection textValues, NameValueCollection imagesValues, NameValueCollection contactValues, NameValueCollection captchaValues)
        {
            try
            {
                // Create a WebRequest for the URI
                WebRequest request = WebRequest.Create(_uri);

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

                ((HttpWebRequest)request).KeepAlive = true;
                ((HttpWebRequest)request).UserAgent = ".NET Framework Client";

                // Stream to Write data to Send
                Stream dataStreamRequest = request.GetRequestStream();

                // Contenido Anuncio
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in textValues.Keys)
                {
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    string formItem = string.Format(formdataTemplate, key, textValues[key]);
                    byte[] formItemBytes = System.Text.Encoding.UTF32.GetBytes(formItem);
                    formItemBytes = Encoding.Convert(utf32, iso, formItemBytes);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                }

                // Images de anuncio. Por default Vacio
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                {
                    string formItem, formImage;
                    byte[] formItemBytes, formImageBytes;
                    // Image A. Max file size.
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                    formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);

                    // Image A. File
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formImage = string.Format(headerTemplate, "ad_picture_a", "", "application/octet-stream");
                    formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                    dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);

                    // Image B. Max file size.
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                    formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);

                    // Image B. File
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formImage = string.Format(headerTemplate, "ad_picture_b", "", "application/octet-stream");
                    formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                    dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);

                    // Image C. Max file size.
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formItem = string.Format(formdataTemplate, "MAX_FILE_SIZE", "307200");
                    formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);

                    // Image C. File
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    formImage = string.Format(headerTemplate, "ad_picture_c", "", "application/octet-stream");
                    formImageBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formImage);
                    dataStreamRequest.Write(formImageBytes, 0, formImageBytes.Length);
                }


                // Informacion de Contacto
                foreach (string key in contactValues.Keys)
                {
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    string formItem = string.Format(formdataTemplate, key, contactValues[key]);
                    byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                }

                // Informacion Captcha
                foreach (string key in captchaValues.Keys)
                {
                    dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);
                    string formItem = string.Format(formdataTemplate, key, captchaValues[key]);
                    byte[] formItemBytes = System.Text.UTF8Encoding.UTF8.GetBytes(formItem);
                    dataStreamRequest.Write(formItemBytes, 0, formItemBytes.Length);
                }

                dataStreamRequest.Write(boundarybytes, 0, boundarybytes.Length);

                // Send Request
                dataStreamRequest.Close();
                ((HttpWebRequest)request).AllowAutoRedirect = false;

                // Obtener respuesta de solicitud
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Found)
                {
                    response.Close();
                    response.Dispose();
                    //estado = AnuncioEstado.Ok;
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.BadGateway)
                {
                    response.Close();
                    response.Dispose();
                    //estado = AnuncioEstado.Revolico;
                    return false;
                }

                response.Close();
                response.Dispose();
                //estado = AnuncioEstado.CaptchaError;
                return false;
            }
            catch (Exception ex)
            {
                //estado = AnuncioEstado.InvalidExecution;
                throw ex;
            }
        }
    }
}
