using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Republish.Models;
using UAParser;
using Republish.Data;
using Republish.Models.Identity;
using Services.Impls;

namespace Republish.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ICustomSignInManger _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public LoginModel(ICustomSignInManger signInManager, UserManager<IdentityUser> userManager, ILogger<LoginModel> logger, ApplicationDbContext context, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl ?? Url.Content("~/");
            TempData["ReturnUrl"] = ReturnUrl;
        }

        public async Task<IActionResult> OnPost([FromForm] InputUsernameModel Input, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                TempData["RememberMe"] = false;

                IdentityUser userTmp = new IdentityUser();
                userTmp.UserName = Input.UserName;

                var result = await _signInManager.SignInAsync(userTmp, false);

                if (result.Succeeded)
                {
                    TempData["UserName"] = userTmp.UserName; ;
                    return new PartialViewResult
                    {
                        ViewName = "LoginPassword",
                        ViewData = new ViewDataDictionary<InputPasswordModel>(ViewData, new InputPasswordModel()),
                    };
                }
                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("User logged in.");
                    return new PartialViewResult
                    {
                        ViewName = "./LoginTwoFactor",
                        ViewData = new ViewDataDictionary<InputTwoFactorModel>(ViewData, new InputTwoFactorModel())
                    };
                }
                if (result.IsLockedOut)
                {
                    return BuildLockoutResult();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return new PartialViewResult
                    {
                        ViewName = "./LoginUserName",
                        ViewData = new ViewDataDictionary<InputUsernameModel>(ViewData, new InputUsernameModel())
                    };
                }
            }

            // If we got this far, something failed, redisplay form
            //return Page();
            return new PartialViewResult
            {
                ViewName = "./LoginUserName",
                ViewData = this.ViewData
            };
        }

        public async Task<IActionResult> OnPostTwoFactor([FromForm] InputTwoFactorModel InputTwoFactor)
        {
            if (!ModelState.IsValid)
            {
                return new PartialViewResult
                {
                    ViewName = "./LoginTwoFactor",
                    ViewData = new ViewDataDictionary<InputTwoFactorModel>(ViewData, new InputTwoFactorModel())
                };
            }

            string returnUrl = (string)TempData.Peek("ReturnUrl");
            bool rememberMe = (bool)TempData.Peek("RememberMe"); ;

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = InputTwoFactor.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);


            var result = await _signInManager.VerifyTwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, InputTwoFactor.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                TempData["RememberMachine"] = InputTwoFactor.RememberMachine;
                TempData["AuthenticatorCode"] = authenticatorCode;

                return new PartialViewResult
                {
                    ViewName = "LoginPassword",
                    ViewData = new ViewDataDictionary<InputPasswordModel>(ViewData, new InputPasswordModel())
                };
            }
            else if (result.IsLockedOut)
            {
                return BuildLockoutResult();
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return new PartialViewResult
                {
                    ViewName = "./LoginTwoFactor",
                    ViewData = new ViewDataDictionary<InputTwoFactorModel>(ViewData, new InputTwoFactorModel())
                };
            }
        }

        public async Task<IActionResult> OnPostPassword([FromForm] InputPasswordModel InputPassword)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                IdentityUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                Microsoft.AspNetCore.Identity.SignInResult result;
                bool UsingTwoFactor = false;

                string returnUrl = (string)TempData.Peek("ReturnUrl");
                bool rememberMe = (bool)TempData.Peek("RememberMe");
                string UserName;

                if (user == null)
                {
                    UsingTwoFactor = false;
                    UserName = (string)TempData.Peek("UserName");
                    result = await _signInManager.PasswordSignInAsync(UserName, InputPassword.Password, false, lockoutOnFailure: true);
                    await UrlRedirect(UserName);
                }
                else // Two-Factor 
                {
                    UsingTwoFactor = true;
                    result = await _signInManager.CheckPasswordSignInAsync(user, InputPassword.Password, lockoutOnFailure: true);
                    UserName = user.UserName;
                }

                if (result.Succeeded)
                {
                    if (UsingTwoFactor == false)
                    {
                        _logger.LogInformation("User logged in.");

                        if (returnUrl == "/")
                        {
                            returnUrl = await UrlRedirect(UserName);
                        }

                        JsonResult js = new JsonResult(new { Url = returnUrl });
                        js.StatusCode = 202;
                        return js;
                    }

                    bool RememberMachine = (bool)TempData.Peek("RememberMachine");
                    string authenticatorCode = (string)TempData["AuthenticatorCode"];

                    var res = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, RememberMachine);

                    if (res.Succeeded)
                    {
                        if (RememberMachine)
                        {
                            await SendNewDeviceAddedEmail(user);
                        }

                        if (returnUrl == "/")
                        {
                            returnUrl = await UrlRedirect(UserName);
                        }

                        _logger.LogInformation("User logged in.");
                        JsonResult js = new JsonResult(new { Url = returnUrl });
                        js.StatusCode = 202;
                        return js;
                    }
                    else if (res.IsLockedOut)
                    {
                        return BuildLockoutResult();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Authenticator code has expired.");
                        return new PartialViewResult
                        {
                            ViewName = "LoginUserName",
                            ViewData = new ViewDataDictionary<InputUsernameModel>(ViewData, new InputUsernameModel())
                        };
                    }
                }
                if (result.IsLockedOut)
                {
                    return BuildLockoutResult();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password Incorrect. Invalid login attempt.");
                    return new PartialViewResult
                    {
                        ViewName = "LoginPassword",
                        ViewData = new ViewDataDictionary<InputPasswordModel>(ViewData, new InputPasswordModel())
                    };
                }
            }

            // If we got this far, something failed, redisplay form
            return new PartialViewResult
            {
                ViewName = "LoginPassword",
                ViewData = new ViewDataDictionary<InputPasswordModel>(ViewData, new InputPasswordModel())
            };
        }

        private async Task SendNewDeviceAddedEmail(IdentityUser user)
        {
            Parser parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(Request.Headers["User-Agent"]);

            string deviceInfo = "<p>A new device has been added for two-factor authentication:</p>";
            deviceInfo += "<p>";
            deviceInfo += "Username: " + user.UserName + "<br />";

            if (!string.IsNullOrEmpty(clientInfo.Device.Family))
            {
                deviceInfo += "Device Name: " + clientInfo.Device.Family + "<br />";
            }

            deviceInfo += "Operating System: " + clientInfo.OS.Family;

            if (!string.IsNullOrEmpty(clientInfo.OS.Major))
            {
                deviceInfo += " " + clientInfo.OS.Major;

                if (!string.IsNullOrEmpty(clientInfo.OS.Minor))
                {
                    deviceInfo += "." + clientInfo.OS.Minor;
                }
            }

            deviceInfo += "<br />";
            deviceInfo += "Browser: " + clientInfo.UA.Family;

            if (!string.IsNullOrEmpty(clientInfo.UA.Major))
            {
                deviceInfo += " " + clientInfo.UA.Major;

                if (!string.IsNullOrEmpty(clientInfo.UA.Minor))
                {
                    deviceInfo += "." + clientInfo.UA.Minor;
                }
            }

            deviceInfo += "</p>";

            await _emailSender.SendEmailAsync(user.Email, "New device added for two-factor authentication", deviceInfo);
        }

        private IActionResult BuildLockoutResult()
        {
            _logger.LogWarning("User account locked out.");
            ModelState.AddModelError(string.Empty, "This account has been locked out. Please reset your password.");
            return new PartialViewResult
            {
                ViewName = "LoginUserName",
                ViewData = new ViewDataDictionary<InputUsernameModel>(ViewData, new InputUsernameModel())
            };
        }

        private async Task<string> UrlRedirect(string UserName)
        {
            return "/Admin/Default";
            IdentityUser userLogin = await _userManager.FindByNameAsync(UserName);

            if(await _userManager.IsInRoleAsync(userLogin, "Admin")){
                return "/SuperUser/Default";
            }
            else if(await _userManager.IsInRoleAsync(userLogin, "Client"))
            {
                return "/SuperUser/Default";
            }

            throw new InvalidOperationException("User: " + UserName + " has invalid user role.");
        }
    }
}
