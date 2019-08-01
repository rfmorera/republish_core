using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class CaptchaKeys
    {
        public CaptchaKeys(string key)
        {
            Id = key;
        }

        [Key]
        [Required]
        public string Id { get; set; }
    }
}
