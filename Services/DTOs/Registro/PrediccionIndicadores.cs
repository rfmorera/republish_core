using Republish.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class PrediccionIndicadores
    {
        public PrediccionIndicadores(double saldoCuenta, double gastoEsperado)
        {
            DateTime utcCuba = DateTime.Now.ToUtcCuba();
            DateTime nextMonth = utcCuba.AddMonths(1);
            nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            TimeSpan diff = nextMonth - utcCuba;
            DiasRestantes = diff.Days + 1;
            GastoAproximado = gastoEsperado;
            double pct = (saldoCuenta / gastoEsperado) * 100;
            DiasDisponible = (int) pct * DiasRestantes / 100;
        }

        public double GastoAproximado { get; set; }
        public int DiasDisponible { get; set; }
        public int DiasRestantes { get; set; }
    }
}
