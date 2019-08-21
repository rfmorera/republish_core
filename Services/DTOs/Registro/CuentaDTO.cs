using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class CuentaDTO
    {
        public CuentaDTO(Cuenta ct)
        {
            Saldo = Math.Round(ct.Saldo, 3);
            LastUpdate = ct.LastUpdate;
        }
        public double Saldo { get; }
        public DateTime? LastUpdate { get; }
    }
}
