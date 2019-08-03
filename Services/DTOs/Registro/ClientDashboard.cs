using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class ClientDashboard
    {
        public ClientDashboard(EstadisticaMensual mensual)
        {
            Mes = mensual;
        }

        public ClientDashboard(EstadisticaDia dia)
        {
            Dia = dia;
        }

        public EstadisticaMensual Mes { get; }
        public EstadisticaDia Dia { get; }

    }
}
