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

        public Task<EstadisticaDiaria> RegistroDiario(string UserId)
        {
            throw new NotImplementedException();
        }

        public Task<EstadisticaMensual> RegistroMensual(string UserId)
        {
            DateTime now = DateTime.Now;
            IEnumerable<Registro> registros = _registroRepo.QueryAll()
                                       .Where(r => r.UserId == UserId && r.DateCreated.Month == now.Month)
                                       .Select(s => s);

            EstadisticaMensual estadistica = new EstadisticaMensual() {AnunciosActualizados = registros.Sum(s => s.AnunciosActualizados) };
            return Task.FromResult(estadistica);
        }
    }
}
