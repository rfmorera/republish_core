using System;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionAppPublicar
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("anunciosq", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            Thread.Sleep(5000);
        }
    }
}
