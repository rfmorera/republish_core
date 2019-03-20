using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IForgotPasswordService
    {
        Task<bool> SendResetPasswordEmail(string userId, string Url, bool createPassword);
        Task<bool> SendResetPasswordEmail(IdentityUser userLogin, string Url, bool createPassword);
    }
}
