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
        Task<Cuenta> GetCuentaIncludeAll(string UserId);
        Task<IEnumerable<string>> FacturarRegistros();
        Task<bool> HasBalance(string UserId);
        Task<double> CostoAnuncio(string UserId);
        Task<double> SetCostoAnuncio(string UserId, double CostoAnuncio);
        Task<IEnumerable<RecargaDetail>> GetRecargasByAgente(string agentId);
        Task<double> GetMontoMesByAgente(string agentId, DateTime date);
        Task<IEnumerable<RecargaDetail>> GetRecargasByClient(string clientId);
        Task<Cuenta> UpdateCuenta(Cuenta cuenta);
    }
}
