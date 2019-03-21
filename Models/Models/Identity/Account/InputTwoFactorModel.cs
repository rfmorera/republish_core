using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Republish.Models.Identity
{
    public class InputTwoFactorModel
    {

        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Código de autenticación")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Recordar")]
        public bool RememberMachine { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
