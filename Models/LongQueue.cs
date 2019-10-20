using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class LongQueue
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
