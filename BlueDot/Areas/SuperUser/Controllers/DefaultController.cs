using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Republish.Data;
using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs;

namespace Republish.Areas.SuperUser.Controllers
{
    [Area("SuperUser")]
    //[Authorize(Roles = "SuperUser")]
    public class DefaultController : Controller
    {


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
    }
}