using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class GrupoEditDTO
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("CantidadAnuncios")]
        public int CantidadAnuncios { get; set; }

        public Grupo UpdateModel(Grupo grupo)
        {
            grupo.Nombre = this.Nombre;
            grupo.Descripcion = this.Descripcion;
            return grupo;
        }
    }
}
