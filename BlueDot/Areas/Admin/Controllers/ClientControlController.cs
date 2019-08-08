using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using Services.DTOs;

namespace RepublishTool.Areas.Admin.Controllers 
{
    [Area(RTRoles.Admin)]
    [Authorize(Roles = RTRoles.Admin)]
    public class ClientControlController : Controller
    {
        private readonly IUserControlService _userControlService;
        public ClientControlController(IUserControlService userControlService)
        {
            _userControlService = userControlService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<UserDTO> model = await _userControlService.GetClientList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string email)
        {
            IdentityUser newuser = new IdentityUser(email);
            await _userControlService.AddClient(newuser);
            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            await _userControlService.RemoveUser(Id);
            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Recargar(string RecargarId, int ValorRecarga)
        {
            if(ValorRecarga <= 0)
            {
                return BadRequest();
            }
            return await BuildPartialView();
        }

        private async Task<IActionResult> BuildPartialView()
        {
            IEnumerable<UserDTO> model = await _userControlService.GetClientList();
            return PartialView(nameof(Index), model);
        }
    }
}