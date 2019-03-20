using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUpdatePasswordService
    {
        Task<IdentityResult> ChangePasswordAsync(UpdatePasswordDTO updatePasswordDTO);
    }
}
