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
            if(a.Enable.HasValue && a.Enable.Value)
            {
                Enable = true;
            }
            else
            {
                Enable = null;
            }
            Title = a.Titulo ?? string.Empty;
            if (!String.IsNullOrEmpty(a.Categoria))
            {
                Categoria = a.Categoria?.ToUpper() ?? string.Empty;
            }
            else
            {
                Categoria = String.Empty;
            }

            if (a.Eliminado)
            {
                BadgeClass = "dark";
                BadgeMessage = "Eliminado";
            }
            else if (a.Procesando != 0)
            {
                BadgeClass = "danger";
                BadgeMessage = "Procesando";
            }
            else if (a.Despublicado)
            {
                BadgeClass = "warning";
                BadgeMessage = "Despublicado";
            }
        }

        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Categoria { get; set; }
        public string BadgeClass { get; set; }
        public string BadgeMessage { get; set; }
        public bool? Enable { get; set; }
    }
}