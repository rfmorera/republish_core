using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class EstadisticaDia
    {
        public EstadisticaDia(int tot)
        {
            Total = tot;
        }

        public int Total { get; }
    }
}
