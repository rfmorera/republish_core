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
        private readonly IEstadisticasService _estadisticasService;
        private readonly UserManager<IdentityUser> _userManager;
        public DefaultController(UserManager<IdentityUser> userManager, IRegistroService registroService, IEstadisticasService estadisticasService)
        {
            _registroService = registroService;
            _userManager = userManager;
            _estadisticasService = estadisticasService;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            ClientDashboard dashboard = await _estadisticasService.GetDashboard(user);
            return View(dashboard);
        }
    }
}