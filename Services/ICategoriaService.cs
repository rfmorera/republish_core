using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoriaService
    {
        Task Add(string UserId, CategoriaIndexDTO categoriaDTO);

        Task<CategoriaDetailsDTO> DetailsAsync(string Id);

        Task Delete(string Id);

        Task Update(CategoriaIndexDTO categoriaUpdatedDTO, string Id);

        Task<IEnumerable<CategoriaIndexDTO>> GetAll(string UserId);
    }
}
