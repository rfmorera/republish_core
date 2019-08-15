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
        private readonly UserManager<IdentityUser> _userManager;
        public DefaultController(UserManager<IdentityUser> userManager, IRegistroService registroService, IUserControlService userControlService)
        {
            _registroService = registroService;
            _userManager = userManager;
            _userControlService = userControlService;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            ClientDashboard dashboard = await _userControlService.GetDashboard(user);
            return View(dashboard);
        }
    }
}