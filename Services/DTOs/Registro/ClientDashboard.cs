using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class ClientDashboard
    {
        public ClientDashboard(double Saldo, EstadisticaDiario dia, EstadisticaSemanal semana, EstadisticaMensual mensual)
        {
            this.Saldo = Saldo;
            Diario = dia;
            Semanal = semana;
            Mensual = mensual;
        }
        
        public double Saldo { get; }
        public EstadisticaDiario Diario { get; }
        public EstadisticaSemanal Semanal { get; }
        public EstadisticaMensual Mensual { get; }
    }
}
