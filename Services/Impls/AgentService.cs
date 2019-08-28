using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlueDot.Data.UnitsOfWorkInterfaces;
using Microsoft.AspNetCore.Identity;
using Services.DTOs;

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

            return result;
        }

        public Task<AgentDetailsDTO> GetAgentDetails(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AgentDTO>> GetAgents()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAgent(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
