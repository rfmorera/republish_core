using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Republish.Models.Identity
{
    public class InputUsernameModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
