using Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IManejadorFinancieroService
    {
        Task InicializarUsuario(string UserId);
        Task RecargarUsuario(RecargaDTO recargaDTO);
        Task<double> GetSaldo(string UserId);
        Task<Cuenta> GetCuenta(string UserId);
        Task<IEnumerable<string>> FacturarRegistros();
        Task<bool> HasBalance(string UserId);
        Task<double> CostoAnuncio(string UserId);
    }
}
