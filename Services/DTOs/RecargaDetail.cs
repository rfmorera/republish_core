using Republish.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class RecargaDetail
    {
        public int Monto { get; set; }

        public string Client { get; set; }

        public DateTime Fecha { get; set; }
        public string FechaAsString { get { return Fecha.ToDateString(); } }
    }
}
