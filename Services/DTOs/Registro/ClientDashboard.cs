using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class ClientDashboard
    {
        public ClientDashboard(EstadisticaDiario dia, EstadisticaSemanal semana)
        {
            Diario = dia;
            Semanal = semana;
        }

        
        public EstadisticaDiario Diario { get; }
        public EstadisticaSemanal Semanal { get; }
        public EstadisticaMensual Mensual { get; }

    }
}
