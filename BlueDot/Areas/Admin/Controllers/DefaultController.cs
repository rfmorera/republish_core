using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Republish.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    [Authorize(Roles = RTRoles.Admin)]
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}