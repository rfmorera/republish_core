using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace RepublishTool.Areas.Client.Controllers
{
    [Area("Client")]
    [AllowAnonymous]
    public class CheckerController : Controller
    {
        private readonly IChequerService _chequerService;
        
        public CheckerController(IChequerService chequerService)
        {
            _chequerService = chequerService;
        }
        
        public async Task<IActionResult> Check()
        {
            try
            {
                string log = await _chequerService.CheckAllTemporizadores();
                return View(model: log);
            }
            catch(Exception ex)
            {
                return View(model: ex.ToString());
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Reset()
        {
            await _chequerService.ResetAll();
            return Ok();
        }
    }
}