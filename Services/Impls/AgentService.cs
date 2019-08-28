using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlueDot.Data.UnitsOfWorkInterfaces;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.DTOs;
using System.Linq;

namespace Services.Impls
{
    public class AgentService : IAgentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public AgentService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IdentityResult> AddAgent(AgentDTO dto)
        {
            IdentityUser agent = new IdentityUser(dto.UserName);
            IdentityResult result = await _userManager.CreateAsync(agent);

            if (!result.Succeeded) return result;
            result = await _userManager.SetPhoneNumberAsync(agent, dto.Phone);

            if (!result.Succeeded) return result;
            result = await _userManager.SetEmailAsync(agent, dto.UserName);

            if (!result.Succeeded) return result;
            result = await _userManager.AddToRoleAsync(agent, RTRoles.Agent);

            return result;
        }

        public Task<AgentDetailsDTO> GetAgentDetails(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AgentDTO>> GetAgents()
        {
            IEnumerable<AgentDTO> list = (await _userManager.GetUsersInRoleAsync(RTRoles.Agent)).Select(a => new AgentDTO(a.Id, a.UserName, a.PhoneNumber));

            return list;
        }

        public async Task<IdentityResult> RemoveAgent(string Id)
        {
            IdentityUser agent = await _userManager.FindByIdAsync(Id);
            IdentityResult result = await _userManager.DeleteAsync(agent);

            return result;
        }
    }
}
