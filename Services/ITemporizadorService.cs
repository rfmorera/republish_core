using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITemporizadorService
    {
        Task AddAsync(TemporizadorDTO temporizadorDTO);

        Task DeleteAsync(string Id);

        Task DeleteAllByGroup(string GrupoId);
    }
}
