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
    
    public class AgentsController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly UserManager<IdentityUser> _userManager;
        public AgentsController(IAgentService agentService, UserManager<IdentityUser> userManager)
        {
            _agentService = agentService;
            _userManager = userManager;
        }

        [Authorize(Roles = RTRoles.Agent)]
        public async Task<IActionResult> Default()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            AgentDetailsDTO model = await _agentService.GetAgentDetails(user.Id);

            return View(model);
        }

        [Authorize(Roles = RTRoles.Admin)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AgentDTO> model = await _agentService.GetAgents();
            return View(model);
        }

        [Authorize(Roles = RTRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add(string UserName, string Phone)
        {
            AgentDTO dTO = new AgentDTO(UserName, Phone);
            IdentityResult result = await _agentService.AddAgent(dTO);
            if (result.Succeeded)
            {
                return await BuildPartialView();
            }
            return BadRequest();
        }

        [Authorize(Roles = RTRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            IdentityResult result = await _agentService.RemoveAgent(Id);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize(Roles = RTRoles.Admin)]
        public async Task<IActionResult> Details(string Id)
        {
            AgentDetailsDTO model = await _agentService.GetAgentDetails(Id);
            
            return View(model);
        }

        [Authorize(Roles = RTRoles.Admin)]
        private async Task<IActionResult> BuildPartialView()
        {
            IEnumerable<AgentDTO> model = await _agentService.GetAgents();
            return PartialView(nameof(Index), model);
        }
    }
}