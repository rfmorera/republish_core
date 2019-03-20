using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Services;
using Services.DTOs;
using Services.Impls;

namespace Republish.Areas.Common.Controllers
{
    [Area("Common")]
    [Authorize]
    public class PasswordController : Controller
    {
        private readonly CustomUserManager _userManager;
        private readonly IUpdatePasswordService _updatePasswordService;
        public PasswordController(CustomUserManager userManager, IUpdatePasswordService updatePasswordService)
        {
            _userManager = userManager;
            _updatePasswordService = updatePasswordService;
        }

        public async Task<IActionResult> Index(string LayoutUrl)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            UpdatePasswordDTO updatePasswordDTO = new UpdatePasswordDTO(user);
            updatePasswordDTO.LayoutUrl = LayoutUrl;
            return View(updatePasswordDTO);
        }

        public IActionResult Success(string LayoutUrl)
        {
            return View(model: LayoutUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdatePasswordDTO updatePasswordDTO)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (updatePasswordDTO.UserLoginId != user.Id)
                {
                    // Error user missmatch
                    ModelState.AddModelError(string.Empty, "User Missmatch");
                    return PartialView("index");
                }

                IdentityResult result = await _updatePasswordService.ChangePasswordAsync(updatePasswordDTO);
                if (result.Succeeded)
                {
                    JsonResult js = new JsonResult(new { Url = "/Common/Password/Success" });
                    js.StatusCode = 202;
                    return js;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return PartialView("Index");
                }
            }
            return PartialView("Index");
        }
    }
}