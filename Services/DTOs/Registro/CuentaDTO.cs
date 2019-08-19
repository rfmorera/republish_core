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
            Saldo = ct.Saldo;
            LastUpdate = ct.LastUpdate;
        }
        public double Saldo { get; }
        public DateTime? LastUpdate { get; }
    }
}
