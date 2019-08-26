using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.DashboardAdmin
{
    public class AdminDashboard
    {
        public AdminDashboard(EstadisticaAnual anual)
        {
            Anual = anual;
        }

        public EstadisticaAnual Anual { get; }
    }
}
