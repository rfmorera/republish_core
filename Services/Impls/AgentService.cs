using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlueDot.Data.UnitsOfWorkInterfaces;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.DTOs;
using System.Linq;
using Republish.Extensions;

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

        public async Task<AgentDetailsDTO> GetAgentDetails(string Id)
        {
            DateTime now = DateTime.Now.ToUtcCuba();
            IdentityUser user = await _userManager.FindByIdAsync(Id);

            double current = (await _unitOfWork.Recarga.FindAllAsync(r => r.OperardorId == Id 
                                                                       && r.DateCreated.Month == now.Month 
                                                                       && r.DateCreated.Year == now.Year))
                                                        .Sum(t => t.Monto);
            now = now.AddMonths(-1);
            double last = (await _unitOfWork.Recarga.FindAllAsync(r => r.OperardorId == Id
                                                           && r.DateCreated.Month == now.Month
                                                           && r.DateCreated.Year == now.Year))
                                            .Sum(t => t.Monto);
            AgentDetailsDTO dto = new AgentDetailsDTO(user, current, last);
            return dto;
        }

        public async Task<IEnumerable<AgentDTO>> GetAgents()
        {
            IEnumerable<AgentDTO> list = (await _userManager.GetUsersInRoleAsync(RTRoles.Agent)).Select(a => new AgentDTO(a));
            IEnumerable<AgentDTO> adminList = (await _userManager.GetUsersInRoleAsync(RTRoles.Admin)).Select(t => new AgentDTO(t));

            list = list.Union(adminList);

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
