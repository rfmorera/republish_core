using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Categoria
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [DefaultValue("")]
        public string Descripcion { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool Activo { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Orden { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public List<Grupo> Grupos { get; set; }
        public Temporizador Temporizador { get; set; }
    }
}
