using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public interface IEstadistica
    {
        double GetTotalGasto();
        int GetTotalAnuncios();
        string ToStringAnuncios();
        string ToStringGastos();
        string ToStringLabels();
    }
}
