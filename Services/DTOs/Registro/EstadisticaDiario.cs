using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaDiario
    {
        public EstadisticaDiario(int tot, double Gasto)
        {
            Total = tot;
            this.Gasto = Gasto;
        }

        public int Total { get; }
        public double Gasto { get; }
    }
}
