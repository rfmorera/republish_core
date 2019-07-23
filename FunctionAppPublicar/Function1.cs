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
            Thread.Sleep(5000);
            List<Task> tasks = new List<Task>();
            IAnuncioService anuncioService = new AnuncioService();

            string key2captcha = "";
            foreach(string st in url)
            {
                tasks.Add(anuncioService.Publish(st, key2captcha));
            }

            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
