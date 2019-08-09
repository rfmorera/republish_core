using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Services.DTOs.Registro
{
    public class EstadisticaMensual : IEstadistica
    {
        public EstadisticaMensual(List<EstadisticaDiario> days, int tot, double gasto)
        {
            Dias = days.OrderBy(d => d.DiaMes).Select(d => d).ToList();
            Total = tot;
            Gasto = gasto;
        }

        public List<EstadisticaDiario> Dias { get; }
        public int Total { get; }
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
            foreach(EstadisticaDiario d in Dias)
            {
                tmp += d.GetTotalAnuncios() + ",";
            }
            return $"[ {tmp.Remove(tmp.Length-1)} ]";
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
