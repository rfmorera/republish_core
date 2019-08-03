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
        Task<EstadisticaDiario> GetDiario(IdentityUser user, DateTime UtcCuba);
        Task<ClientDashboard> GetDashboard(IdentityUser user);
    }
}
