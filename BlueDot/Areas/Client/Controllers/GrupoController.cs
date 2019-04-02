using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs;

namespace RepublishTool.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class GrupoController : Controller
    {
        private readonly IGrupoService _grupoService;
        public GrupoController(IGrupoService grupoService)
        {
            _grupoService = grupoService;
        }

        public async Task<IActionResult> Details(string GrupoId)
        {
            GrupoDetailsDTO model = await _grupoService.DetailsAsync(GrupoId);
            return View(model);
        }

        public async Task<IActionResult> DeleteAllAnuncios(string GrupoId)
        {
            return View();
        }
    }
}