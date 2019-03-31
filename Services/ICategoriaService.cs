using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoriaService
    {
        Task Add(CategoriaDTO categoriaDTO);

        Task Remove(string Id);

        Task Update(CategoriaDTO categoriaUpdatedDTO, string Id);
    }
}
