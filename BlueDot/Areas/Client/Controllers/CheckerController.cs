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
                await _chequerService.CheckAllTemporizadores();
                return Ok();
            }
            catch
            {
                return BadRequest();
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