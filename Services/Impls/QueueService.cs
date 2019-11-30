using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Services.Impls
{
    public class QueueService : IQueueService
    {
        private readonly IRepository<ShortQueue> _queueRepository;
        readonly ILogger _log;

        public QueueService(ApplicationDbContext context, ILogger<QueueService> log)
        {
            _queueRepository = new Repository<ShortQueue>(context);
            _log = log;
        }

        public async Task Add(string id, DateTime dateTime)
        {
            ShortQueue queue = new ShortQueue();
            queue.Url = id;
            queue.Created = dateTime;
            await _queueRepository.AddAsync(queue);
        }
    }
}
