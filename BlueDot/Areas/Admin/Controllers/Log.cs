using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Republish.Data;
using Republish.Extensions;
using Services;
using Services.DTOs.DashboardAdmin;

namespace Republish.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    [Authorize(Roles = RTRoles.Admin)]
    public class LogController : Controller
    {
        public LogController()
        {
        }

        public IActionResult Index()
        {
            DateTime now = DateTime.Now.ToUtcCuba();
            using (FileStream logFileStream = new FileStream($"logger{now.Year}{now.Month}{now.Day}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader logFileReader = new StreamReader(logFileStream))
            {
                string text = logFileReader.ReadToEnd();
                string[] model = text.Split("\n");
                return View(model);
            }
        }

    }
}