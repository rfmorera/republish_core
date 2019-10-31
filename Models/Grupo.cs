using Microsoft.AspNetCore.Identity;
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
        [Key]
        public string Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Orden { get; set; }

        [Required]
        public bool Activo { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; }

        public bool Enable { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public List<Temporizador> Temporizadores { get; set; }
        public List<Anuncio> Anuncios { get; set; }
    }
}
