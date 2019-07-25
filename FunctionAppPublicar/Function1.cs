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
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            string[] url = myQueueItem.Split(";");
            
            List<Task> tasks = new List<Task>();
            IAnuncioService anuncioService = new AnuncioService();
            string key2Captcha = "fda47557683d8da30367c38b6e8756de";
            foreach(string st in url)
            {
                tasks.Add(anuncioService.Publish(st, key2Captcha));
            }

            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
