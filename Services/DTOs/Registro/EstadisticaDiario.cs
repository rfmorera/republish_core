using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Linq;

namespace Services.DTOs.Registro
{
    public class EstadisticaDiario : IEstadistica
    {
        public EstadisticaDiario(IEnumerable<Models.Registro> registros, DateTime utcCuba)
        {
            Total = registros.Sum(r => r.CaptchasResuletos);
            Gasto = Math.Round(registros.Sum(r => r.Gasto), 3);
            registros = registros.OrderBy(r => r.DateCreated);
            
            List<IGrouping<int, Models.Registro>> groups = registros.GroupBy(r => r.DateCreated.Hour).ToList();

            DiaMes = utcCuba.Day;

            Horas = groups.Select(r => r.Key).ToArray();
            GastoHoras = groups.Select(r => r.ToList().Sum(t => t.Gasto)).ToArray();
            AnunciosHoras = groups.Select(r => r.ToList().Sum(t => t.AnunciosActualizados)).ToArray();
        }

        public int DiaMes { get; }
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
            int len = Horas.Length, val;
            string tmp = "[";
            for(int i = 0; i < len; i++)
            {
                val = Horas[i];
                if(val > 12)
                {
                    val -= 12;
                    tmp += "\"" + val + "pm\",";
                }
                else
                {
                    tmp += "\"" + val + "am\",";
                }
                
            }
            tmp = tmp.Substring(0, tmp.Length-1) + "]";
            return tmp;
        }
    }
}
