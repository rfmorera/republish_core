using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGrupoService
    {
        Task AddAsync(GrupoIndexDTO grupoDTO);

        Task DeleteAsync(string Id);

        void Publish(string Id, int Etapa,string t);

        Task<GrupoDetailsDTO> DetailsAsync(string Id);

        Task<IEnumerable<GrupoIndexDTO>> GetAllAsync(string UserId);
    }
}
