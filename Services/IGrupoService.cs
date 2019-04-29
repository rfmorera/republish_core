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

        Task<IEnumerable<AnuncioDTO>> Select(string Id, int Etapa, string t);

        Task Publish(IEnumerable<AnuncioDTO> list);

        Task<GrupoDetailsDTO> DetailsAsync(string Id);

        Task<IEnumerable<GrupoIndexDTO>> GetAllAsync(string UserId);
    }
}
