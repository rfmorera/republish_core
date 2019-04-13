using System;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Launcher
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("1 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            WebRequest request = WebRequest.Create("https://localhost:44368/Client/Grupo/CheckTemporizadores");
            ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
            request.Credentials = CredentialCache.DefaultCredentials;
            //Console.WriteLine(((HttpWebRequest)request).ServicePoint.Address.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if(response.StatusCode == HttpStatusCode.Accepted)
            {
                log.Info($"Chequeo Ok!!");
            }
            else
            {
                log.Info($"~~~ Bad ~~~ Chequeo incorrecto");
            }
        }
    }
}
