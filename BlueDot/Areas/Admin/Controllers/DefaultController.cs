using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Republish.Data;
using Services;
using Services.DTOs.DashboardAdmin;

namespace Republish.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    
    public class DefaultController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEstadisticaAdminService _estadisticaAdminService;

        public DefaultController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext, IEstadisticaAdminService estadisticaAdminService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _estadisticaAdminService = estadisticaAdminService;
        }

        [Authorize(Roles = RTRoles.Admin)]
        public IActionResult Index()
        {
            AdminDashboard model = _estadisticaAdminService.GetDashboard();
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Init()
        {
            IdentityRole ad = new IdentityRole(RTRoles.Admin);
            ad.NormalizedName = RTRoles.Admin.ToUpper();
            await _dbContext.Roles.AddAsync(ad);

            IdentityRole cl = new IdentityRole(RTRoles.Client);
            cl.NormalizedName = RTRoles.Client.ToUpper();
            await _dbContext.Roles.AddAsync(cl);
            
            await _dbContext.AddAsync(new CaptchaKeys("none"));

            IdentityUser raf = new IdentityUser("rfmorera@gmail.com");
            await _userManager.CreateAsync(raf, "Ciber*2019");
            
            raf = await _userManager.FindByNameAsync("rfmorera@gmail.com");

            await _userManager.AddToRoleAsync(raf, RTRoles.Admin);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}