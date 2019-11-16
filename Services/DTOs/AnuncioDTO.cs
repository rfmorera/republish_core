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
            Enable = a.Enable;
            Title = a.Titulo ?? string.Empty;
            if (!String.IsNullOrEmpty(a.Categoria))
            {
                Categoria = a.Categoria?.ToUpper() ?? string.Empty;
            }
            else
            {
                Categoria = String.Empty;
            }
        }

        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Categoria { get; set; }
        public bool Enable { get; set; }
    }
}
