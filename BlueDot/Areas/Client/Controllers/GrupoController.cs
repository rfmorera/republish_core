using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.DTOs;

namespace RepublishTool.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class GrupoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGrupoService _grupoService;
        private readonly IAnuncioService _anuncioService;
        private readonly ITemporizadorService _temporizadorService;
        private readonly IChequerService _chequerService;
        readonly ILogger<GrupoController> _log;

        public GrupoController(UserManager<IdentityUser> userManager, IGrupoService grupoService, IAnuncioService anuncioService, ITemporizadorService temporizadorService, IChequerService chequerService, ILogger<GrupoController> log)
        {
            _grupoService = grupoService;
            _userManager = userManager;
            _anuncioService = anuncioService;
            _temporizadorService = temporizadorService;
            _chequerService = chequerService;
            _log = log;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<GrupoIndexDTO> grupoIndexDTOs = await _grupoService.GetAllAsync(user.Id);
            return View(grupoIndexDTOs);
        }
        
        public async Task<IActionResult> Details(string GrupoId)
        {
            GrupoDetailsDTO model = await _grupoService.DetailsAsync(GrupoId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GrupoIndexDTO grupoIndexDTO)
        {

            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            grupoIndexDTO.UserId = user.Id;
            await _grupoService.AddAsync(grupoIndexDTO);

            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string GrupoId)
        {
            await _grupoService.DeleteAsync(GrupoId);

            return await BuildPartialView();
        }

        public async Task<IActionResult> Publish(string GrupoId)
        {
            _grupoService.Publish(GrupoId, 20, "Manual");

            return await Index();
        }

        [HttpPost]
        public async Task<IActionResult> AddAnuncio(string GrupoId, string Enlaces)
        {
            string[] list = Enlaces.Split("\r\n");
            await _anuncioService.AddAsync(GrupoId, list);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAnuncio(string GrupoId, string AnuncioId)
        {
            await _anuncioService.DeleteAsync(AnuncioId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllAnuncios(string GrupoId)
        {
            await _anuncioService.DeleteAllByGroup(GrupoId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> AddTemporizador(TemporizadorDTO dTO)
        {
            await _temporizadorService.AddAsync(dTO);

            return await BuildPartialDetailsView(dTO.GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTemporizador(string GrupoId, string TemporizadorId)
        {
            await _temporizadorService.DeleteAsync(TemporizadorId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllTemporizadores(string GrupoId)
        {
            await _temporizadorService.DeleteAllByGroup(GrupoId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [AllowAnonymous]
        //[HttpPost]
        public IActionResult CheckTemporizadores()
        {
            _chequerService.CheckAllTemporizadores();
            return Ok();
        }

        [AllowAnonymous]
        //[HttpPost]
        public async Task<IActionResult> ResetTemporizadores()
        {
            await _chequerService.ResetAll();
            return Ok();
        }
        private async Task<IActionResult> BuildPartialView()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<GrupoIndexDTO> grupoIndexDTOs = await _grupoService.GetAllAsync(user.Id);
            return PartialView("Index", grupoIndexDTOs);
        }

        private async Task<IActionResult> BuildPartialDetailsView(string GrupoId)
        {
            GrupoDetailsDTO grupoDetails = await _grupoService.DetailsAsync(GrupoId);
            return PartialView("Details", grupoDetails);
        }
    }
}