using Microsoft.AspNetCore.Identity;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAgentService
    {
        Task<IEnumerable<AgentDTO>> GetAgents();
        Task<AgentDetailsDTO> GetAgentDetails(string Id);
        Task<IdentityResult> AddAgent(AgentDTO agent);
        Task RemoveAgent(string Id);
    }
}
