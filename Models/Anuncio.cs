using System;
using System.Collections.Generic;
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

        [Required]
        public string GroupId { get; set; }
        public Grupo Grupo { get; set; }
    }
}
