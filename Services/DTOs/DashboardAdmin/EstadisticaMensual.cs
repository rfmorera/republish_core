using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.DashboardAdmin
{
    public class EstadisticaMensual : IEstadisticaAdmin
    {
        public EstadisticaMensual(DateTime date, double Gasto, double Venta)
        {
            this.Gasto = Gasto;
            this.Venta = Venta;
            this.Fecha = date;
        }

        public double Gasto { get; }

        public double Venta { get; }

        public DateTime Fecha { get; }

        public string ToStringGastos()
        {
            throw new NotImplementedException();
        }

        public string ToStringLabel()
        {
            throw new NotImplementedException();
        }

        public string ToStringVentas()
        {
            throw new NotImplementedException();
        }
    }
}
