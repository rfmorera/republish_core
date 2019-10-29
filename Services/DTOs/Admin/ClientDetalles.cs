using Services.DTOs.Registro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Admin
{
    public class ClientDetalles
    {
        public ClientDetalles(ClientDashboard Dashboard, IEnumerable<RecargaDetail> Recargas)
        {
            this.Dashboard = Dashboard;
            this.Recargas = Recargas;
        }

        public ClientDashboard Dashboard { get; set; }
        public IEnumerable<RecargaDetail> Recargas { get; set; }
    }
}
