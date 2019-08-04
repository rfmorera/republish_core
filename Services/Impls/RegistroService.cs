using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs.Registro;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Services.Impls
{
    public class RegistroService : IRegistroService
    {
        private readonly IRepository<Registro> _registroRepo;
        public RegistroService(ApplicationDbContext context)
        {
            _registroRepo = new Repository<Registro>(context);
        }

        public async Task Registro(Registro registro)
        {
            await _registroRepo.AddAsync(registro);
        }

        public async Task AddRegistros(IEnumerable<Registro> registros)
        {
            await _registroRepo.AddAllAsync(registros);
        }

        public Task<EstadisticaDiario> RegistroDiario(string UserId)
        {
            throw new NotImplementedException();
        }
    }
}
