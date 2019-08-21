using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IClienteOpcionesService
    {
        Task<ClienteOpciones> GetOpciones(string UserId);
        Task<bool> TemporizadorStatus(string UserId);
        Task<bool> TemporizadorStatus(string UserId, bool status);
    }
}
