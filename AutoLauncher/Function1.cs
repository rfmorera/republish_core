using System;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AutoLauncher
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log/*, IChequerService chequerService*/)
        {
            try
            {
                log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
                WebRequest request = WebRequest.Create("https://localhost:44306/Client/Grupo/CheckTemporizadores");
                //request.Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    log.Info($"Chequeo Ok!!");
                }
                else
                {
                    log.Info($"~~~ Bad ~~~ Chequeo incorrecto");
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
            }
        }
    }
}
