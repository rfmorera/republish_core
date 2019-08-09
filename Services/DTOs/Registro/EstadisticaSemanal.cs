using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaSemanal : IEstadistica
    {
        public EstadisticaSemanal(List<EstadisticaDiario> last7Days, int total, DateTime utcCuba, double Gasto)
        {
            this.Dias = last7Days;
            this.Total = total;
            this.Inicio = utcCuba.AddDays(-6);
            this.Fin = utcCuba;
            this.Gasto = Gasto;
        }

        public List<EstadisticaDiario> Dias { get; set; }
        public int Total { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public double Gasto { get; }

        public int GetTotalAnuncios()
        {
            return Total;
        }

        public double GetTotalGasto()
        {
            return Gasto;
        }

        public string ToStringAnuncios()
        {
            string tmp = "";
            foreach (EstadisticaDiario d in Dias)
            {
                tmp += d.GetTotalAnuncios() + ",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }

        public string ToStringGastos()
        {
            string tmp = "";
            foreach (EstadisticaDiario d in Dias)
            {
                tmp += d.GetTotalGasto() + ",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }

        public string ToStringLabels()
        {
            string tmp = "";
            foreach (EstadisticaDiario d in Dias)
            {
                tmp += d.DiaMes + ",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }
    }
}
