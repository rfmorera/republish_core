using Republish.Data;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Services.Impls
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly CustomUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public ForgotPasswordService(ApplicationDbContext context, CustomUserManager userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<bool> SendResetPasswordEmail(IdentityUser user, string baseUrl, bool createPassword)
        {
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return true;
            }

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["code"] = code;
            string callbackUrl = baseUrl + "?" + query.ToString();

            string resetInfo = "", subject ="";
            if (createPassword)
            {
                resetInfo = "<p>Password create instructions:</p>";
                resetInfo += "<p>";
                resetInfo += "You have received this email because a Republish administrator account has been created for you.";
                resetInfo += "</p>";

                resetInfo += "<p>";
                resetInfo += $"Please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a> to set your password and access the Administrator module.";
                resetInfo += "</p>";
                subject = "Create Password";
            }
            else
            {
                resetInfo = "<p>Password reset instructions:</p>";
                resetInfo += "<p>";
                resetInfo += "You have received this mail because you (or someone else) have asked to reset your password.";
                resetInfo += "</p>";

                resetInfo += "<p>";
                resetInfo += $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                resetInfo += "</p>";
                subject = "Reset Password";
            }
            
            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                resetInfo);

            return true;
        }

        public async Task<bool> SendResetPasswordEmail(string userId, string baseUrl, bool createPassword)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            return await SendResetPasswordEmail(user, baseUrl, createPassword);
        }
    }
}
