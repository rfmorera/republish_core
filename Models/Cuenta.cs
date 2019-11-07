using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Cuenta
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public double Saldo { get; set; }

        public double CostoAnuncio { get; set; }

        [Required]
        public DateTime UltimaRecarga { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
