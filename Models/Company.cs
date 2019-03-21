using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Republish.Models
{
    public class Company
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string ExternalName { get; set; }
        public int ExternalPort { get; set; }
        public string FromEmail { get; set; }
        [StringLength(50)]
        public string ExternalUsername { get; set; }
        [StringLength(500)]
        public string ExternalPassword { get; set; }
        public int ExternalTimeout { get; set; }
    }
}
