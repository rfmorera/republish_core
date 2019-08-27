using Services.DTOs.DashboardAdmin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEstadisticaAdminService
    {
        AdminDashboard GetDashboard();
        EstadisticaAnual GetEstadisticaAnual();
    }
}
