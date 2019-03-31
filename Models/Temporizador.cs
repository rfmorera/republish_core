using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Temporizador
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }

        [Required]
        [ForeignKey("ForeignKey_Categoria_Temporizador")]
        public string CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
