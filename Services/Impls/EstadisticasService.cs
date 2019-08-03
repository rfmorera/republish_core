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

            ClientDashboard dashboard = new ClientDashboard(dia);
            return dashboard;
        }

        public async Task<EstadisticaDiario> GetDiario(IdentityUser user, DateTime UtcCuba)
        {
            IEnumerable<Registro> registros = await repositoryRegistro.FindAllAsync(r => 
                                                                                    (r.DateCreated.DayOfYear == UtcCuba.DayOfYear 
                                                                                    && r.DateCreated.Year == UtcCuba.Year
                                                                                    && r.UserId == user.Id));
            int tot = registros.Sum(r => r.CaptchasResuletos);
            EstadisticaDiario dia = new EstadisticaDiario(tot);
            return dia;
        }
    }
}
