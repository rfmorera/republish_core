﻿using Microsoft.AspNetCore.Identity;
using Models;
using Republish.Data;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Registro;

namespace Services.Impls
{
    public class UserControlService : IUserControlService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEstadisticasService _estadisticasService;
        private readonly IGrupoService _grupoService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly IManejadorFinancieroService _financieroService;
        
        public UserControlService(UserManager<IdentityUser> userManager, IEstadisticasService estadisticasService, IManejadorFinancieroService financieroService, IGrupoService grupoService, ITemporizadorService temporizadorService)
        {
            _userManager = userManager;
            _estadisticasService = estadisticasService;
            _financieroService = financieroService;
            _grupoService = grupoService;
            _temporizadorService = temporizadorService;
        }

        public async Task<IdentityResult> AddAdmin(IdentityUser user)
        {
            IdentityResult result = await _userManager.CreateAsync(user, "Servicios*2019");
            if (!result.Succeeded)
            {
                return result;
            }
            result = await _userManager.AddToRoleAsync(user, RTRoles.Admin);
            return result;
        }

        public async Task<IdentityResult> AddClient(IdentityUser user)
        {
            IdentityResult result = await _userManager.CreateAsync(user, "Servicios*2019");
            if (!result.Succeeded)
            {
                return result;
            }
            await _financieroService.InicializarUsuario(user.Id);
            result = await _userManager.AddToRoleAsync(user, RTRoles.Client);
            return result;
        }

        public async Task<IdentityResult> LockoutUser(IdentityUser user, bool flag)
        {
            return await _userManager.SetLockoutEnabledAsync(user, flag);
        }

        public async Task<IdentityResult> RemoveUser(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<UserDTO>> GetClientList()
        {
            IEnumerable<UserDTO> list = (await _userManager.GetUsersInRoleAsync(RTRoles.Client))
                                                .Select(t => new UserDTO(t.Id, t.UserName, t.Email, RTRoles.Client));

            return list;
        }

        public async Task<ClientDashboard> GetDashboard(IdentityUser user)
        {
            EstadisticaDiario dia = await _estadisticasService.GetDiario(user);
            EstadisticaSemanal semana = await _estadisticasService.GetSemanal(user);
            EstadisticaMensual mensual = await _estadisticasService.GetMensual(user);

            Cuenta cnt = await _financieroService.GetCuenta(user.Id);

            ClientDashboard dashboard = new ClientDashboard(cnt, dia, semana, mensual);
            return dashboard;
        }

        public async Task CheckOutCeroBalanceAccount()
        {
            IEnumerable<string> listUsers = await _financieroService.FacturarRegistros();
            List<Task> tasksList = new List<Task>();
            foreach(string UserId in listUsers)
            {
                tasksList.Add(_temporizadorService.SetEnable(UserId, false));
            }

            await Task.WhenAll(tasksList);
        }

        public async Task RecargarCliente(RecargaDTO recargaDTO)
        {
            await _financieroService.RecargarUsuario(recargaDTO);
            await _temporizadorService.SetEnable(recargaDTO.ClientId, true);
        }
    }
}
