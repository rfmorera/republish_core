using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs.Registro;
using System.Linq;
using Republish.Extensions;

namespace Services.Impls
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Registro> repositoryRegistro;

        public EstadisticasService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryRegistro = new Repository<Registro>(dbContext);
        }

        public async Task<ClientDashboard> GetDashboard(IdentityUser user)
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            EstadisticaDiario dia = await GetDiario(user, UtcCuba);
            EstadisticaSemanal semana = await GetSemanal(user, UtcCuba);
            EstadisticaMensual mensual = await GetMensual(user, UtcCuba);

            ClientDashboard dashboard = new ClientDashboard(dia, semana, mensual);
            return dashboard;
        }

        public async Task<EstadisticaDiario> GetDiario(IdentityUser user, DateTime UtcCuba)
        {
            IEnumerable<Registro> registros = await repositoryRegistro.FindAllAsync(r => 
                                                                                    (r.DateCreated.DayOfYear == UtcCuba.DayOfYear 
                                                                                    && r.DateCreated.Year == UtcCuba.Year
                                                                                    && r.UserId == user.Id));
            EstadisticaDiario dia = new EstadisticaDiario(registros, UtcCuba);
            return dia;
        }

        public async Task<EstadisticaMensual> GetMensual(IdentityUser user, DateTime UtcCuba)
        {
            List<EstadisticaDiario> days = new List<EstadisticaDiario>();
            while(UtcCuba.Day > 1)
            {
                days.Add(await GetDiario(user, UtcCuba));
                UtcCuba = UtcCuba.AddDays(-1);
            }
            days.Add(await GetDiario(user, UtcCuba));

            int tot = days.Sum(r => r.Total);
            double gasto = days.Sum(r => r.Gasto);
            EstadisticaMensual mes = new EstadisticaMensual(days, tot, gasto);
            return mes;
        }

        public async Task<EstadisticaSemanal> GetSemanal(IdentityUser user, DateTime UtcCuba)
        {
            List<EstadisticaDiario> last7Days = new List<EstadisticaDiario>();
            for(int i = 6; i >= 0; i--)
            {
                last7Days.Add(await GetDiario(user, UtcCuba.AddDays(-i)));
            }

            int total = last7Days.Sum(d => d.Total);
            double gasto = last7Days.Sum(d => d.Gasto);
            EstadisticaSemanal semana = new EstadisticaSemanal(last7Days, total, UtcCuba, gasto);

            return semana;
        }
    }
}
