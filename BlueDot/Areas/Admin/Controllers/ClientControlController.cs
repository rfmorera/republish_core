using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Republish.Extensions;
using Services;
using Services.DTOs;
using Services.DTOs.Admin;
using Services.DTOs.Registro;

namespace RepublishTool.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    [Authorize(Roles = RTRoles.Admin + "," + RTRoles.Agent)]
    public class ClientControlController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserControlService _userControlService;
        private readonly IManejadorFinancieroService _financieroService;
        private readonly INotificationsService _notificationsService;
        private readonly IEstadisticasService _estadisticasService;

        public ClientControlController(UserManager<IdentityUser> userManager, IUserControlService userControlService, IManejadorFinancieroService financieroService, INotificationsService notificationsService, IEstadisticasService estadisticasService)
        {
            _userControlService = userControlService;
            _userManager = userManager;
            _financieroService = financieroService;
            _notificationsService = notificationsService;
            _estadisticasService = estadisticasService;
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
        public async Task<IActionResult> Recargar(string ClientId, int ValorRecarga, bool detallesView)
        {
            if (ValorRecarga <= 0)
            {
                return BadRequest();
            }
            IdentityUser user = await _userManager.GetUserAsync(User);

            RecargaDTO r = new RecargaDTO(user.Id, ClientId, ValorRecarga);
            await _userControlService.RecargarCliente(r);
            if (detallesView)
            {
                ClientDetalles model = await BuildDetallesModel(ClientId);
                return PartialView(nameof(Detalles), model);
            }
            return await BuildPartialView();
        }

        private async Task<IActionResult> BuildPartialView()
        {
            IEnumerable<UserDTO> model = await _userControlService.GetClientList();
            return PartialView(nameof(Index), model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(string ClientId)
        {
            ClientDetalles model = await BuildDetallesModel(ClientId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CostoAnuncio(string ClientId, double CostoAnuncio)
        {
            if (CostoAnuncio < 0.003)
            {
                return BadRequest();
            }

            await _financieroService.SetCostoAnuncio(ClientId, CostoAnuncio);

            ClientDetalles model = await BuildDetallesModel(ClientId);
            return PartialView(nameof(Detalles), model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotificacion(string ClientId, string Mensaje)
        {
            if (Mensaje.Length < 5)
            {
                return BadRequest();
            }

            Notificacion notificacion = new Notificacion()
            {
                UserId = ClientId,
                Mensaje = Mensaje,
                Readed = false,
                DateCreated = DateTime.Now.ToUtcCuba()
            };

            await _notificationsService.Add(notificacion);

            return Ok();
        }

        private async Task<ClientDetalles> BuildDetallesModel(string ClientId)
        {
            Cuenta cuenta = await _financieroService.GetCuentaIncludeAll(ClientId);
            IdentityUser clientUser = cuenta.User;
            IEnumerable<RecargaDetail> recargas = await _financieroService.GetRecargasByClient(ClientId);

            DateTime date = DateTime.Now.ToUtcCuba();
            double GastoEsperadoActual = await _userControlService.GetGastoEsperadoByClient(ClientId, date);
            PrediccionIndicadores prediccion = new PrediccionIndicadores(cuenta.Saldo, GastoEsperadoActual);

            date = date.AddMonths(1);
            date = new DateTime(date.Year, date.Month, 1);
            double GastoEsperadoProximo = await _userControlService.GetGastoEsperadoByClient(ClientId, date); ;

            EstadisticaDiario dia = await _estadisticasService.GetDiario(ClientId);
            EstadisticaSemanal semana = await _estadisticasService.GetSemanal(ClientId);
            EstadisticaMensual mensual = await _estadisticasService.GetMensual(ClientId);

            ClientDetalles model = new ClientDetalles(clientUser, recargas, cuenta,
                                                      prediccion, GastoEsperadoProximo,
                                                      dia, semana, mensual);
            return model;
        }
    }
}