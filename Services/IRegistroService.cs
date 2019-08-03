using Models;
using Services.DTOs.Registro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IRegistroService
    {
        Task Registro(Registro registro);
        Task AddRegistros(IEnumerable<Registro> registros);
        Task<EstadisticaDia> RegistroDiario(string UserId);
        Task<EstadisticaMensual> RegistroMensual(string UserId);
    }
}
