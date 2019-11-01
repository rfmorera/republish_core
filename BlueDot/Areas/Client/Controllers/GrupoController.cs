using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
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
        private readonly IManejadorFinancieroService _financieroService;
        readonly ILogger<GrupoController> _log;

        public GrupoController(UserManager<IdentityUser> userManager, IGrupoService grupoService, IAnuncioService anuncioService, ITemporizadorService temporizadorService, IChequerService chequerService, ILogger<GrupoController> log, IManejadorFinancieroService financieroService)
        {
            _grupoService = grupoService;
            _userManager = userManager;
            _anuncioService = anuncioService;
            _temporizadorService = temporizadorService;
            _chequerService = chequerService;
            _log = log;
            _financieroService = financieroService;
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
        public async Task<IActionResult> Edit(GrupoEditDTO dto)
        {
            Grupo grupo = await _grupoService.GetAsync(dto.Id);
            grupo = dto.UpdateModel(grupo);

            await _grupoService.UpdateAsync(grupo);

            return Json(dto);
        }

        [HttpPost]
        public async Task<IActionResult> EnableGrupo(string Id)
        {
            await _grupoService.ToogleEnable(Id);
            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string GrupoId)
        {
            await _grupoService.DeleteAsync(GrupoId);

            return Ok(GrupoId);
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

            return Ok(AnuncioId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllAnuncios(string GrupoId)
        {
            await _anuncioService.DeleteAllByGroup(GrupoId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAnuncioTitle(string GrupoId)
        {
            await _anuncioService.UpdateTitle(GrupoId);

            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> AddTemporizador(TemporizadorDTO dTO)
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (dTO.IntervaloHoras + dTO.IntervaloMinutos <= 0) return BadRequest();

            Temporizador t = dTO.BuildModel(user);
            await _temporizadorService.AddAsync(t, await _financieroService.HasBalance(user.Id));

            return await BuildPartialDetailsView(dTO.GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTemporizador(string GrupoId, string TemporizadorId)
        {
            await _temporizadorService.DeleteAsync(TemporizadorId);

            return Ok(TemporizadorId);
        }

        [HttpPost]
        public async Task<IActionResult> ToogleTemporizador(string Id, string GrupoId)
        {
            Temporizador t = await _temporizadorService.TooggleEnable(Id);
            return await BuildPartialDetailsView(GrupoId);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllTemporizadores(string GrupoId)
        {
            await _temporizadorService.DeleteAllByGroup(GrupoId);

            return await BuildPartialDetailsView(GrupoId);
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