using Models;
using Newtonsoft.Json;
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

        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("CantidadAnuncios")]
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

        public Grupo UpdateModel(Grupo grupo)
        {
            grupo.Nombre = this.Nombre;
            grupo.Descripcion = this.Descripcion;
            return grupo;
        }
    }
}
