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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IRepository<Registro> _registroRepository;
        private IRepository<Recarga> _recargaRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<Registro> Registro
        {
            get
            {
                return _registroRepository = _registroRepository ?? new Repository<Registro>(_dbContext);
            }
        }

        public IRepository<Recarga> Recarga
        {
            get
            {
                return _recargaRepository = _recargaRepository ?? new Repository<Recarga>(_dbContext);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
