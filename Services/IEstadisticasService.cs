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
        Task<EstadisticaDia> GetDia(IdentityUser user);
        Task<ClientDashboard> GetDashboard(IdentityUser user);
    }
}
