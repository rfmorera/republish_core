using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Services.DTOs;

namespace Services.Impls
{
    public class QueueService : IQueueService
    {
        private readonly CloudQueueClient _cloudQueueClient;
        private readonly CloudQueue _cloudQueue;

        public QueueService(CloudQueueClient cloudQueueClient)
        {
            _cloudQueueClient = cloudQueueClient;
            _cloudQueue = _cloudQueueClient.GetQueueReference("anunciosq");
        }

        public async Task AddMessageAsync(string captchaKey, IEnumerable<AnuncioDTO> list)
        {
            string msgStr = captchaKey + ";";
            
            foreach(AnuncioDTO dt in list)
            {
                msgStr += dt.Url + ";";
            }

            CloudQueueMessage message = new CloudQueueMessage(msgStr);

            await _cloudQueue.AddMessageAsync(message);
        }
    }
}
