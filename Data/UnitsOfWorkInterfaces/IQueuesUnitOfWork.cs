using Models;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueDot.Data.UnitsOfWorkInterfaces
{
    public interface IQueuesUnitOfWork
    {
        IRepository<ShortQueue> Short { get; }
        IRepository<LongQueue> Long { get; }
        Task SaveChangesAsync();
    }
}
