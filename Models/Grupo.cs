using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Grupo
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        [Required]
        public string CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public List<Anuncio> Anuncios { get; set; }
    }
}
