using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Linq;

namespace Services.DTOs.Registro
{
    public class EstadisticaDiario : IEstadistica
    {
        public EstadisticaDiario(IEnumerable<Models.Registro> registros)
        {
            Total = registros.Sum(r => r.CaptchasResuletos);
            Gasto = registros.Sum(r => r.Gasto);
            registros = registros.OrderBy(r => r.DateCreated);
            
            List<IGrouping<int, Models.Registro>> groups = registros.GroupBy(r => r.DateCreated.Hour).ToList();

            Horas = groups.Select(r => r.Key).ToArray();
            GastoHoras = groups.Select(r => r.ToList().Sum(t => t.Gasto)).ToArray();
            AnunciosHoras = groups.Select(r => r.ToList().Sum(t => t.AnunciosActualizados)).ToArray();
        }

        public int Total { get; }
        public double Gasto { get; }
        public int[] AnunciosHoras { get; }
        public double[] GastoHoras { get; }
        public int[] Horas { get; }

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
            string tmp = $"[ {String.Join(",", AnunciosHoras)} ]"; 
            return tmp;
        }

        public string ToStringGastos()
        {
            string tmp = $"[ {String.Join(",", GastoHoras)} ]";
            return tmp;
        }

        public string ToStringLabels()
        {
            string tmp = $"[ {String.Join(",", Horas)} ]";
            return tmp;
        }
    }
}
