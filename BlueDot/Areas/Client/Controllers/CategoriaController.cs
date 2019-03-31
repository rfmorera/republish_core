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
        public CategoriaController(UserManager<IdentityUser> userManager, ICategoriaService categoriaService)
        {
            _userManager = userManager;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<CategoriaIndexDTO> categoriaDTOs = await _categoriaService.GetAll(user.Id);
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

        private async Task<IActionResult> BuildPartialView()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<CategoriaIndexDTO> categoriaDTOs = await _categoriaService.GetAll(user.Id);
            return PartialView("Index", categoriaDTOs);
        }
    }
}