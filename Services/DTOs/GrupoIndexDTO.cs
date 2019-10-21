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
            Descripcion = grupo.Descripcion;
            CantidadAnuncios = grupo.Anuncios.Count;
            UserId = grupo.UserId;
            Activo = grupo.Activo;
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadAnuncios { get; set; }
        public bool Activo { get; set; }
        public string UserId { get; set; }

        internal Grupo BuildModel()
        {
            Grupo grupo = new Grupo();
            grupo.Nombre = Nombre;
            grupo.UserId = UserId;
            grupo.Descripcion = Descripcion;
            return grupo;
        }
    }
}
