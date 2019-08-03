using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaDiario
    {
        public EstadisticaDiario(int tot)
        {
            Total = tot;
        }

        public int Total { get; }
    }
}
