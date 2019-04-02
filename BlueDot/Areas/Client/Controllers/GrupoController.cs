using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public GrupoController(UserManager<IdentityUser> userManager, IGrupoService grupoService, IAnuncioService anuncioService)
        {
            _grupoService = grupoService;
            _userManager = userManager;
            _anuncioService = anuncioService;
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
            await _grupoService.AddAsync(grupoIndexDTO);

            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string GrupoId)
        {
            await _grupoService.DeleteAsync(GrupoId);

            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddAnuncio(string GrupoId, IEnumerable<string> enlaces)
        {
            await _anuncioService.AddAsync(enlaces);

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