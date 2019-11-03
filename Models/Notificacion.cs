using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Notificacion
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public string Mensaje { get; set; }

        public bool Readed { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
