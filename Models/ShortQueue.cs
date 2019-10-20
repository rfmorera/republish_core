using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ShortQueue
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Url { get; set; }

        public DateTime? Created { get; set; }
    }
}
