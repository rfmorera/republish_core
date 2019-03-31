using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class CategoriaDetailsDTO
    {
        public CategoriaDetailsDTO(Categoria categoria, IEnumerable<GrupoIndexDTO> list)
        {
            Id = categoria.Id;
            Nombre = categoria.Nombre;
            Grupos = list;
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public IEnumerable<GrupoIndexDTO> Grupos { get; set; }
    }
}
