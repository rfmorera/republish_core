using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace RepublishTool.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    [Authorize(Roles = RTRoles.Admin)]
    public class CaptchaServerController : Controller
    {
        private readonly ICaptchaService _captchaService;
        public CaptchaServerController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        public async Task<IActionResult> Index()
        {
            CaptchaKeys model = await _captchaService.GetCaptchaKeyAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string ApiKey)
        {
            await _captchaService.Update2CaptchaKey(ApiKey);
            CaptchaKeys model = await _captchaService.GetCaptchaKeyAsync();
            return PartialView(nameof(Index), model);
        }
    }
}