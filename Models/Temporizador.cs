﻿using Microsoft.AspNetCore.Identity;
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

        public string Nombre { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Orden { get; set; }

        public bool Enable { get; set; }
        public bool UserEnable { get; set; }
        public bool SystemEnable { get; set; }

        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        
        public TimeSpan NextExecution { get; set; }

        public int IntervaloHoras { get; set; }
        public int IntervaloMinutos { get; set; }

        public int Etapa { get; set; }

        [Required]
        public string GrupoId { get; set; }
        public Grupo Grupo { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
