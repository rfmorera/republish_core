using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Services.DTOs
{
    public class UpdatePasswordDTO
    {
        public UpdatePasswordDTO()
        {

        }

        public UpdatePasswordDTO(IdentityUser user)
        {
            this.UserLoginId = user.Id;
        }

        [Required]
        public string UserLoginId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        public string LayoutUrl { get; set; }
    }
}
