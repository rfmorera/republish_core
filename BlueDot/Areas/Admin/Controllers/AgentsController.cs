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
    public class AgentsController : Controller
    {
        private readonly IAgentService _agentService;
        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AgentDTO> model = await _agentService.GetAgents();
            return View(model);
        }

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

        public async Task<IActionResult> Details(string Id)
        {
            AgentDetailsDTO model = await _agentService.GetAgentDetails(Id);
            
            return View(model);
        }

        private async Task<IActionResult> BuildPartialView()
        {
            IEnumerable<AgentDTO> model = await _agentService.GetAgents();
            return PartialView(nameof(Index), model);
        }
    }
}