using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RepublishTool.Areas.Client.Controllers
{
    public class GrupoController : Controller
    {
        public async Task<IActionResult> Delete(string GrupoId)
        {
            return View();
        }
    }
}