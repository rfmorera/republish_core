using Microsoft.AspNetCore.Identity;
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
using Services.Extensions;

namespace Services.Impls
{
    public class UserControlService : IUserControlService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEstadisticasService _estadisticasService;
        private readonly IGrupoService _grupoService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly IManejadorFinancieroService _financieroService;
        private readonly IClienteOpcionesService _opcionesService;
        
        public UserControlService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEstadisticasService estadisticasService, IManejadorFinancieroService financieroService, IGrupoService grupoService, ITemporizadorService temporizadorService, IClienteOpcionesService opcionesService)
        {
            _context = context;
            _userManager = userManager;
            _estadisticasService = estadisticasService;
            _financieroService = financieroService;
            _grupoService = grupoService;
            _temporizadorService = temporizadorService;
            _opcionesService = opcionesService;
        }

        public async Task<IdentityResult> AddAdmin(IdentityUser user)
        {
            IdentityResult result = await _userManager.CreateAsync(user, "Ciber*2019");
            if (!result.Succeeded)
            {
                return result;
            }
            result = await _userManager.AddToRoleAsync(user, RTRoles.Admin);
            await _opcionesService.InicializarUsuario(user.Id);
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
            await _opcionesService.InicializarUsuario(user.Id);
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
                                                .Join(_context.Cuenta,
                                                      u => u.Id,
                                                      c => c.UserId,
                                                      (u, c) => new UserDTO(u.Id, u.UserName, u.Email, RTRoles.Client, c.Saldo));


            return list;
        }

        public async Task<ClientDashboard> GetDashboard(IdentityUser user)
        {
            EstadisticaDiario dia = await _estadisticasService.GetDiario(user);
            EstadisticaSemanal semana = await _estadisticasService.GetSemanal(user);
            EstadisticaMensual mensual = await _estadisticasService.GetMensual(user);

            Cuenta cnt = await _financieroService.GetCuenta(user.Id);
            ClienteOpciones opt = await _opcionesService.GetOpciones(user.Id);
            double costoAnuncio = await _financieroService.CostoAnuncio(user.Id);
            double gastoEsperado = (await _grupoService.GetByUser(user.Id))
                                                       .Sum(g => g.Temporizadores
                                                                    .Sum(t => t.Costo(costoAnuncio, g.Anuncios.Count)));


            PrediccionIndicadores prediccion = new PrediccionIndicadores(cnt.Saldo, gastoEsperado);

            ClientDashboard dashboard = new ClientDashboard(cnt, dia, semana, mensual, opt, prediccion);
            return dashboard;
        }

        public async Task CheckOutCeroBalanceAccount()
        {
            IEnumerable<string> listUsers = await _financieroService.FacturarRegistros();
            List<Task> tasksList = new List<Task>();
            foreach(string UserId in listUsers)
            {
                tasksList.Add(_temporizadorService.SetSystemEnable(UserId, false));
            }

            await Task.WhenAll(tasksList);
        }

        public async Task RecargarCliente(RecargaDTO recargaDTO)
        {
            await _financieroService.RecargarUsuario(recargaDTO);
            await _temporizadorService.SetSystemEnable(recargaDTO.ClientId, true);
        }
    }
}
