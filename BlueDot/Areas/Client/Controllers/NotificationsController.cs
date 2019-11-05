using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Services;
using Services.DTOs;
using Services.Extensions;

namespace RepublishTool.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class NotificationsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INotificationsService _notificationsService;
        
        public NotificationsController(UserManager<IdentityUser> userManager, INotificationsService notificationsService)
        {
            _userManager = userManager;
            _notificationsService = notificationsService;
        }
        
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<NotificacionDTO> notificacions = (await _notificationsService.GetByUser(user.Id)).ToNotificacionDTO();
            await _notificationsService.SetReadedByUser(user.Id);
            return View(notificacions);
        }
    }
}