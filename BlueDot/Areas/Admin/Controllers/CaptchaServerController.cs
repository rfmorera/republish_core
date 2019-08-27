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
            IEnumerable<CaptchaKeys> model = await _captchaService.GetCaptchaKeyAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string Key, string Account)
        {
            CaptchaKeys captcha = new CaptchaKeys(Key, Account);
            await _captchaService.Add(captcha);
            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            await _captchaService.Delete(Id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Id, string ApiKey)
        {
            await _captchaService.Update2CaptchaKey(Id, ApiKey);
            return await BuildPartialView();
        }

        private async Task<IActionResult> BuildPartialView()
        {
            IEnumerable<CaptchaKeys> model = await _captchaService.GetCaptchaKeyAsync();
            return PartialView(nameof(Index), model);
        }
    }
}