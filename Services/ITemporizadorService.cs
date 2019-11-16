using Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITemporizadorService
    {
        Task AddAsync(Temporizador temporizador, bool SystemEnable);

        Task DeleteAsync(string Id);

        Task<IEnumerable<Temporizador>> GetByGroup(string GroupId);

        Task<IEnumerable<Temporizador>> GetByUser(string userId);

        Task DeleteAllByGroup(string GrupoId);

        Task SetSystemEnable(string UserId, bool SystemEnable);
        Task<bool> ToogleUserEnable(string UserId);
        Task<Temporizador> TooggleEnable(string Id);
        Task<IEnumerable<Temporizador>> GetRunning();
    }
}
