using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaSemanal
    {
        public EstadisticaSemanal(List<EstadisticaDiario> last7Days, int total, DateTime utcCuba, double Gasto)
        {
            this.Dias = last7Days;
            this.Total = total;
            this.Inicio = utcCuba.AddDays(-7);
            this.Fin = utcCuba;
            this.Gasto = Gasto;
        }

        public List<EstadisticaDiario> Dias { get; set; }
        public int Total { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public double Gasto { get; }
    }
}
