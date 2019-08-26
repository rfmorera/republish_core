using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Models;

namespace Services.DTOs.DashboardAdmin
{
    public class EstadisticaAnual : IEstadisticaAdmin
    {
        public EstadisticaAnual(List<EstadisticaMensual> list)
        {
            Gasto = list.Sum(m => m.Gasto);
            Venta = list.Sum(m => m.Venta);
            Meses = list.OrderBy(m => m.Fecha).ToList();
        }

        public double Gasto { get; }
        public double Venta { get; }
        public List<EstadisticaMensual> Meses { get; }

        public DateTime Fecha { get; set; }

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
                tmp += MonthOfYear.Meses[m.Fecha.Month] + ",";
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
