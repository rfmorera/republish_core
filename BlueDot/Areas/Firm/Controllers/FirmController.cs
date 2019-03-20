using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Republish.Areas.Firm.Controllers
{
    [Area("Firm")]
    [Authorize(Roles = "AttorneyParalegal")]
    public class FirmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}