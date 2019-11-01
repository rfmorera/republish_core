using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHostingEnvironment _env;
        public LogController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            try
            {
                DateTime now = DateTime.Now.ToUtcCuba();
                string day = (now.Day < 10 ? "0" : String.Empty) + now.Day;
                string month = (now.Month < 10 ? "0" : String.Empty) + now.Month;
                string filePath = System.IO.Path.Combine(_env.ContentRootPath, $"logger{now.Year}{month}{day}");
                using (FileStream logFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader logFileReader = new StreamReader(logFileStream))
                {
                    string text = logFileReader.ReadToEnd();
                    string[] model = text.Split("\n");
                    return View(model);
                }
            }
            catch(Exception ex)
            {
                string[] model = { ex.Message, ex.StackTrace, ex.Source };
                return View(model);
            }
        }

    }
}