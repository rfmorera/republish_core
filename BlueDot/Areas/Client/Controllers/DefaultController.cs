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
        private readonly UserManager<IdentityUser> _userManager;
        public DefaultController(UserManager<IdentityUser> userManager, IRegistroService registroService)
        {
            _registroService = registroService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            EstadisticaMensual estadistica = await _registroService.RegistroMensual(user.Id);
            ClientDashboard dashboard = new ClientDashboard(estadistica);
            return View(dashboard);
        }
    }
}