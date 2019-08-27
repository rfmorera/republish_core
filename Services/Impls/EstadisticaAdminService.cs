using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs.DashboardAdmin;
using System.Linq;
using Republish.Extensions;

namespace Services.Impls
{
    public class EstadisticaAdminService : IEstadisticaAdminService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Recarga> repositoryRecarga;
        public EstadisticaAdminService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryRecarga = new Repository<Recarga>(dbContext);
        }

        public AdminDashboard GetDashboard()
        {
            EstadisticaAnual anual = GetEstadisticaAnual();

            AdminDashboard dashboard = new AdminDashboard(anual);
            return dashboard;
        }

        public EstadisticaAnual GetEstadisticaAnual()
        {
            DateTime now = DateTime.Now.ToUtcCuba();
            IEnumerable<EstadisticaMensual> list = from m in repositoryRecarga.QueryAll()
                                                   where m.DateCreated.Year == now.Year
                                                   group m by m.DateCreated.Month into est
                                                   select new EstadisticaMensual(est.First().DateCreated.Year, est.First().DateCreated.Month, 0, est.Sum(p => p.Monto));

            EstadisticaAnual anual = new EstadisticaAnual(list);
            return anual;
        }
    }
}
