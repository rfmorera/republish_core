using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Registro
    {
        public Registro()
        {

        }

        public Registro(string userId, int capResueltos, DateTime dateTime, double costo)
        {
            UserId = userId;
            CaptchasResuletos = capResueltos;
            AnunciosActualizados = capResueltos;
            DateCreated = dateTime;
            Gasto = costo * capResueltos;
        }

        [Key]
        [Required]
        public string Id { get; set; }
        
        public DateTime DateCreated { get; set; }

        public int CaptchasResuletos { get; set; }
        public int AnunciosActualizados { get; set; }
        public double Gasto { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
