using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ClienteOpciones
    {
        public ClienteOpciones(string UserId)
        {
            this.UserId = UserId;
            TemporizadoresUserEnable = true;
        }
        [Key]
        public string Id { get; set; }

        public bool TemporizadoresUserEnable { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
