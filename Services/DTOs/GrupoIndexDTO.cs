using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class GrupoIndexDTO
    {
        public GrupoIndexDTO()
        {

        }

        public GrupoIndexDTO(Grupo grupo)
        {
            Id = grupo.Id;
            Nombre = grupo.Nombre;
            //CantidadAnuncios = grupo.Anuncios.Count;
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public int CantidadAnuncios { get; set; } = 15;

        public string CategoriaId { get; set; }

        internal Grupo BuildModel()
        {
            Grupo grupo = new Grupo();
            grupo.Nombre = Nombre;
            grupo.CategoriaId = CategoriaId;
            return grupo;
        }
    }
}
