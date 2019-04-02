using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class GrupoDetailsDTO
    {
        private Grupo grupo;

        public GrupoDetailsDTO(Grupo grupo, IEnumerable<AnuncioDTO> list)
        {
            this.grupo = grupo;
            Id = grupo.Id;
            Nombre = grupo.Nombre;
            CategoriaId = grupo.CategoriaId;
            CategoriaNombre = grupo.Categoria.Nombre;
            Anuncios = list;
        }

        public string Id { get; set; }
        public string Nombre { get; set; }

        public string CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }

        public IEnumerable<AnuncioDTO> Anuncios { get; set; }
    }
}
