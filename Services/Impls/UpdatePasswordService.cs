using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Services.DTOs;

namespace Services.Impls
{
    public class UpdatePasswordService : IUpdatePasswordService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UpdatePasswordService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ChangePasswordAsync(UpdatePasswordDTO updatePasswordDTO)
        {
            IdentityUser userLogin = await _userManager.FindByIdAsync(updatePasswordDTO.UserLoginId);
            IdentityResult result = await _userManager.ChangePasswordAsync(userLogin, 
                                                                           updatePasswordDTO.CurrentPassword,
                                                                           updatePasswordDTO.NewPassword);
            return result;
        }
    }
}
