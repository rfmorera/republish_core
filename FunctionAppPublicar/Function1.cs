using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Services;
using Services.Impls;

namespace FunctionAppPublicar
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("anunciosq", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            string[] url = myQueueItem.Split(";");
            log.LogInformation($"C# Queue trigger function processed. -- Total#: {url.Length} -- ");
            
            List<Task> tasks = new List<Task>();
            IAnuncioService anuncioService = new AnuncioService(log);
            string key2Captcha = url[0];
            foreach(string st in url)
            {
                tasks.Add(anuncioService.Publish(st, key2Captcha));
            }

            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
