﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Anuncio
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Url { get; set; }

        [NotMapped]
        public Uri UrlFormat { get
            {
                return new Uri(Url);
            }
            set
            {
                Url = value.ToString();
            }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Orden { get; set; }

        public bool Caducado { get; set; }

        public bool Actualizado { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; }

        [Required]
        public string GroupId { get; set; }
        public Grupo Grupo { get; set; }
    }
}
