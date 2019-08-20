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
        Task AddAsync(TemporizadorDTO temporizadorDTO, bool Enable);

        Task DeleteAsync(string Id);

        Task<IEnumerable<Temporizador>> GetByGroup(string GroupId);

        Task DeleteAllByGroup(string GrupoId);

        Task SetEnable(string Id, bool status);
        Task SetEnable(IEnumerable<Temporizador> lst, bool status);
    }
}
