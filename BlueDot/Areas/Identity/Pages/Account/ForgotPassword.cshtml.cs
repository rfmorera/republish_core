using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Republish.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Impls;
using Services;
using Republish.Models.Identity;

namespace Republish.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IForgotPasswordService forgotPasswordService)
        {
            _userManager = userManager;
            _forgotPasswordService = forgotPasswordService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(Input.Email);
                string callbackUrl = Url.Page(
                            pageName: "/Account/ResetPassword",
                            pageHandler: null,
                            protocol: Request.Scheme,
                            values: new { area = "Identity" }
                            );

                await _forgotPasswordService.SendResetPasswordEmail(user.Id, callbackUrl, false);
                
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
