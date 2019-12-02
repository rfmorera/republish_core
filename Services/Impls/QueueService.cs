using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Republish.Extensions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services.Impls
{
    public class queueService : IQueueService
    {
        private readonly IRepository<ShortQueue> _queueRepository;
        readonly ILogger _log;
        private readonly ApplicationDbContext _context;

        public queueService(ApplicationDbContext context, ILogger<queueService> log)
        {
            _queueRepository = new Repository<ShortQueue>(context);
            _log = log;
            _context = context;
        }

        public async Task Add(string id, DateTime dateTime)
        {
            ShortQueue queue = new ShortQueue();
            queue.AnuncioId = id;
            queue.Created = dateTime;
            await _queueRepository.AddAsync(queue);
        }

        public async Task<IEnumerable<Anuncio>> GetAnunciosFromQueue()
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            IEnumerable<Anuncio> anunciosFromQueue = await (from q in _context.ShortQueue
                                                            where q.Created <= UtcCuba
                                                            join a in _context.Anuncio on q.AnuncioId equals a.Id
                                                            select a).ToListAsync();

            _queueRepository.RemoveRange(await _queueRepository.FindAllAsync(q => q.Created <= UtcCuba));
            await _queueRepository.SaveChangesAsync();

            return anunciosFromQueue;
        }
    }
}
