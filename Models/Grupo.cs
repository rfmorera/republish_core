using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Grupo
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Orden { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool Activo { get; set; }

        [Required]
        public string CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public List<Anuncio> Anuncios { get; set; }
    }
}
