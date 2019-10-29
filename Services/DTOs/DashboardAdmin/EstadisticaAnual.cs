using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Models;

namespace Services.DTOs.DashboardAdmin
{
    public class EstadisticaAnual : IEstadisticaAdmin
    {
        public EstadisticaAnual(IEnumerable<EstadisticaMensual> list)
        {
            Gasto = list.Sum(e => e.Gasto);
            Venta = list.Sum(e => e.Venta);
            Meses = list.OrderBy(o => o.Year).OrderBy(o => o.Month).ToList();
            if (list.Any())
            {
                Year = list.First().Year;
            }
            else
            {
                Year = 0;
            }
        }

        public double Gasto { get; }
        public double Venta { get; }
        public List<EstadisticaMensual> Meses { get; }

        public string Fecha => $"{Year}";

        private int Year { get; set; }

        public string ToStringGastos()
        {
            string tmp = "";
            foreach (EstadisticaMensual m in Meses)
            {
                tmp += m.Gasto + ",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }

        public string ToStringLabel()
        {
            string tmp = "";
            foreach (EstadisticaMensual m in Meses)
            {
                tmp += "\"" + MonthOfYear.Meses[m.Month - 1] + "\",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }

        public string ToStringVentas()
        {
            string tmp = "";
            foreach (EstadisticaMensual m in Meses)
            {
                tmp += m.Venta + ",";
            }
            return $"[ {tmp.Remove(tmp.Length - 1)} ]";
        }
    }
}
