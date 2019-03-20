using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Republish.Models.Identity
{
    public class InputPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
