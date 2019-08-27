using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.DashboardAdmin
{
    public class EstadisticaMensual : IEstadisticaAdmin, IComparable<EstadisticaMensual>
    {
        public EstadisticaMensual(int year, int month, double Gasto, double Venta)
        {
            this.Gasto = Gasto;
            this.Venta = Venta;
            Year = year;
            Month = month;
        }

        public double Gasto { get; }

        public double Venta { get; }

        public int Year { get; }
        public int Month { get; }

        public string Fecha => $"{Year}-{Month}";

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

        public int CompareTo(EstadisticaMensual est)
        {
            if(Year != est.Year)
            {
                return Year - est.Year;
            }

            return Month - est.Month;
        }
    }
}
