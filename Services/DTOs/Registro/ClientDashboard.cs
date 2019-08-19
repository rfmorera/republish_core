using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class ClientDashboard
    {
        public ClientDashboard(Cuenta ct, EstadisticaDiario dia, EstadisticaSemanal semana, EstadisticaMensual mensual)
        {
            Cnt = new CuentaDTO(ct);
            Diario = dia;
            Semanal = semana;
            Mensual = mensual;
        }
        
        public CuentaDTO Cnt { get; }
        public EstadisticaDiario Diario { get; }
        public EstadisticaSemanal Semanal { get; }
        public EstadisticaMensual Mensual { get; }
    }
}
