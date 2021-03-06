﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlueDot.Data.UnitsOfWorkInterfaces;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Republish.Extensions;

namespace Services.Impls
{
    public class AgentService : IAgentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IManejadorFinancieroService _financieroService;
        public AgentService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IManejadorFinancieroService financieroService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _financieroService = financieroService;
        }

        public async Task<IdentityResult> AddAgent(AgentDTO dto)
        {
            IdentityUser agent = new IdentityUser(dto.UserName);
            IdentityResult result = await _userManager.CreateAsync(agent, "Servicios*2019");

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

            double current = await _financieroService.GetMontoMesByAgente(Id, now);

            now = now.AddMonths(-1);

            double last = await _financieroService.GetMontoMesByAgente(Id, now);

            IEnumerable<RecargaDetail> recargas = await _financieroService.GetRecargasByAgente(Id);


            AgentDetailsDTO dto = new AgentDetailsDTO(user, current, last, recargas);
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
            if(!(await _userManager.IsInRoleAsync(agent, RTRoles.Agent)))
            {
                IdentityResult result = await _userManager.DeleteAsync(agent);

                return result;
            }

            throw new Exception("Los administradores no pueden ser eliminados");
        }
    }
}
