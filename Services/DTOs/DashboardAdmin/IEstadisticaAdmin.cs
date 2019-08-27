using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.DashboardAdmin
{
    public interface IEstadisticaAdmin
    {
        string Fecha { get; }
        double Gasto { get; }
        double Venta { get; }
        string ToStringVentas();
        string ToStringGastos();
        string ToStringLabel();
    }
}
