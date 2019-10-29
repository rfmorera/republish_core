using Microsoft.AspNetCore.Identity;
using Services.DTOs.Registro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEstadisticasService
    {
        Task<EstadisticaDiario> GetDiario(string clientId);
        Task<EstadisticaSemanal> GetSemanal(string clientId);
        Task<EstadisticaMensual> GetMensual(string clientId);
    }
}
