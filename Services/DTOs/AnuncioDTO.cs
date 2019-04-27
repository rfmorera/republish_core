using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class AnuncioDTO
    {
        public AnuncioDTO(Anuncio a)
        {
            Id = a.Id;
            Url = a.Url;
            Caducado = a.Actualizado;
        }

        public string Id { get; set; }
        public string Url { get; set; }
        public bool Caducado { get; set; }
    }
}
