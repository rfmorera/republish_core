using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs.Registro;

namespace RepublishTool.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class DefaultController : Controller
    {
        private readonly IRegistroService _registroService;
        private readonly IUserControlService _userControlService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly UserManager<IdentityUser> _userManager;

        public DefaultController(UserManager<IdentityUser> userManager, IRegistroService registroService, IUserControlService userControlService, ITemporizadorService temporizadorService)
        {
            _registroService = registroService;
            _userManager = userManager;
            _userControlService = userControlService;
            _temporizadorService = temporizadorService;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            ClientDashboard dashboard = await _userControlService.GetDashboard(user);
            return View(dashboard);
        }

        [HttpPost]
        public async Task<IActionResult> TemporizadorUserEnable()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            bool UserEnable = await _temporizadorService.ToogleUserEnable(user.Id);
            return PartialView("~/Areas/Client/Views/Common/_onoffswitch.cshtml", UserEnable);
        }
    }
}