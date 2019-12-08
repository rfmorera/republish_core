using Newtonsoft.Json;
using Services.DTOs;
using Services.DTOs.AnuncioHelper;
using Services.Impls;
using Services.Results;
using System;
using System.IO;

namespace RestoreAnuncios
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            AnuncioService anuncioService = new AnuncioService(null, null, null, null);

            try
            {
                using (StreamReader reader = new StreamReader("C:\\Users\\Rafael Fernanadez\\source\\repos\\RepublishCore\\RestoreAnuncios\\source.txt"))
                using (StreamWriter writer = new StreamWriter("C:\\Users\\Rafael Fernanadez\\source\\repos\\RepublishCore\\RestoreAnuncios\\target.txt"))
                {
                    string url;
                    int cnt = 1;
                    while (!reader.EndOfStream && (url = reader.ReadLine()).Length > 0)
                    {
                        Console.WriteLine(cnt);
                        try
                        {
                            FormInsertAnuncio formInsertAnuncio = anuncioService.Retrieve(url).GetAwaiter().GetResult();
                            string jsonForm = $"{JsonConvert.SerializeObject(formInsertAnuncio)}";

                            writer.WriteLine(jsonForm);
                        }
                        catch (Exception)
                        {
                            writer.WriteLine(" no " + url);
                        }
                        cnt++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            using (StreamReader reader = new StreamReader("C:\\Users\\Rafael Fernanadez\\source\\repos\\RepublishCore\\RestoreAnuncios\\target.txt"))
            using (StreamWriter writer = new StreamWriter("C:\\Users\\Rafael Fernanadez\\source\\repos\\RepublishCore\\RestoreAnuncios\\links-modificar.txt"))
            {
                string line;
                int cnt = 1;
                while (!reader.EndOfStream && (line = reader.ReadLine()).Length > 0)
                {
                    Console.WriteLine(cnt);
                    try
                    {
                        FormInsertAnuncio formInsertAnuncio = JsonConvert.DeserializeObject<FormInsertAnuncio>(line);
                        //Captcha
                        string html = "DS\", \"RECAPTCHA_V2_SITE_KEY\":\"6LfyRCIUAAAAAP5zhuXfbwh63Sx4zqfPmh3Jnjy7\",\"RECAPTCHA_V3_SITE_KEY\":\"6Lfw";
                        CaptchaAnswer captchaAnswer = anuncioService.ResolveCaptcha("91092bd5c3c38f309e077e595d94226c", "https://www.revolico.com/insertar-anuncio.html", html).GetAwaiter().GetResult();
                        formInsertAnuncio.variables.captchaResponse = captchaAnswer.Answerv2;
                        formInsertAnuncio.variables.botScore = captchaAnswer.Answerv3;

                        string answer = anuncioService.InsertAnuncio(formInsertAnuncio).GetAwaiter().GetResult();
                        InsertResult insertResult = anuncioService.ParseInsertResult(answer);
                        writer.WriteLine("https://www.revolico.com/modificar-anuncio.html?key=" + insertResult.FullId);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine(" no " + cnt);
                        Console.WriteLine(ex.Message);
                    }
                    cnt++;
                }
            }
        }
    }
}
