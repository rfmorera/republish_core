using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Recarga
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int Monto { get; set; }

        [Required]
        public string ClientId { get; set; }
        public IdentityUser Client { get; set; }

        [Required]
        public string OperardorId { get; set; }
        public IdentityUser Operardor { get; set; }
    }
}
