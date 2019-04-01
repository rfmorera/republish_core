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
    public class CategoriaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICategoriaService _categoriaService;
        private readonly IGrupoService _grupoService;
        public CategoriaController(UserManager<IdentityUser> userManager, ICategoriaService categoriaService, IGrupoService grupoService)
        {
            _userManager = userManager;
            _categoriaService = categoriaService;
            _grupoService = grupoService;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<CategoriaIndexDTO> categoriaDTOs = await _categoriaService.GetAllAsync(user.Id);
            return View(categoriaDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoriaIndexDTO categoriaDTO)
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            await _categoriaService.Add(user.Id, categoriaDTO);

            return await BuildPartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string CategoriaId)
        {
            await _categoriaService.Delete(CategoriaId);

            return await BuildPartialView();
        }

        public async Task<IActionResult> Details(string CategoriaId)
        {
            CategoriaDetailsDTO model = await _categoriaService.DetailsAsync(CategoriaId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddGrupo(GrupoIndexDTO grupoIndexDTO)
        {
            await _grupoService.AddAsync(grupoIndexDTO);

            CategoriaDetailsDTO model = await _categoriaService.DetailsAsync(grupoIndexDTO.CategoriaId);

            return PartialView("Details", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGrupo(string CategoriaId, string GrupoId)
        {
            await _grupoService.DeleteAsync(GrupoId);

            CategoriaDetailsDTO model = await _categoriaService.DetailsAsync(CategoriaId);

            return PartialView("Details", model);
        }

        private async Task<IActionResult> BuildPartialView()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<CategoriaIndexDTO> categoriaDTOs = await _categoriaService.GetAllAsync(user.Id);
            return PartialView("Index", categoriaDTOs);
        }
    }
}