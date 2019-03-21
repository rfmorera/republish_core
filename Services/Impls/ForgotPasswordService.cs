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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public ForgotPasswordService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
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
                resetInfo = "<p>Intruscciones para crear contraseña:</p>";
                resetInfo += "<p>";
                resetInfo += "Usted a recibido este correo porque se ha creado una cuenta de cliente en Republish Tool para usted.";
                resetInfo += "</p>";

                resetInfo += "<p>";
                resetInfo += $"Por favor <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>de clic aquí</a> para establecer su contraseña y acceder al modulo de Cliente.";
                resetInfo += "</p>";
                subject = "Crear Contraseña";
            }
            else
            {
                resetInfo = "<p>Intruscciones para reiniciar contraseña:</p>";
                resetInfo += "<p>";
                resetInfo += "Usted ha recibido este correo porque usted (o alguien mas) ha solicitado reiniciar su contraseña.";
                resetInfo += "</p>";

                resetInfo += "<p>";
                resetInfo += $"Por favor reinicie su contraseña haciendo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicj aquí</a>.";
                resetInfo += "</p>";
                subject = "Reiniciar Contraseña";
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
