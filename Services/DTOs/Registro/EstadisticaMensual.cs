using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaMensual
    {
        public EstadisticaMensual(List<EstadisticaDiario> days, int tot, double gasto)
        {
            Dias = days;
            Total = tot;
            Gasto = gasto;
        }

        public List<EstadisticaDiario> Dias { get; }
        public int Total { get; }
        public double Gasto { get; }
    }
}
