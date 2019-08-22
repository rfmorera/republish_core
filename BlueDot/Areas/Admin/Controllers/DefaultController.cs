using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Republish.Data;

namespace Republish.Areas.Admin.Controllers
{
    [Area(RTRoles.Admin)]
    
    public class DefaultController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public DefaultController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Authorize(Roles = RTRoles.Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Init()
        {
            await _dbContext.Roles.AddAsync(new IdentityRole(RTRoles.Admin));
            await _dbContext.Roles.AddAsync(new IdentityRole(RTRoles.Client));

            IdentityUser raf = new IdentityUser("rfmorera@gmail.com");
            await _userManager.CreateAsync(raf, "Ciber*2019");
            
            raf = await _userManager.FindByNameAsync("rfmorera@gmail.com");

            await _userManager.AddToRoleAsync(raf, RTRoles.Admin);

            return Ok();
        }
    }
}