using BlueDot.Data.UnitsOfWorkInterfaces;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueDot.Data.UnitsOfWork
{
    public class QueuesUnitOfWork : IQueuesUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IRepository<ShortQueue> repositoryShortQueue;
        private IRepository<LongQueue> repositoryLongQueue;

        public QueuesUnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<ShortQueue> Short
        {
            get
            {
                return repositoryShortQueue = repositoryShortQueue ?? new Repository<ShortQueue>(_dbContext);
            }
        }

        public IRepository<LongQueue> Long
        {
            get
            {
                return repositoryLongQueue = repositoryLongQueue ?? new Repository<LongQueue>(_dbContext);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
