using Models;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueDot.Data.UnitsOfWorkInterfaces
{
    public interface IUnitOfWork
    {
        IRepository<Registro> Registro { get; }
        IRepository<Recarga> Recarga { get; }
        Task SaveChangesAsync();
    }
}
