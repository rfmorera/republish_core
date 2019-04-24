using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Registro
    {
        [Key]
        [Required]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int CaptchasResuletos { get; set; }
        public int AnunciosActualizados { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
