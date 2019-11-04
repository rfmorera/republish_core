using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Services.Impls
{
    public class NotificationsService : INotificationsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Notificacion> repositoryNotificacion;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            repositoryNotificacion = new Repository<Notificacion>(context);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Notificacion>> GetByCurrentUser()
        {
            IdentityUser user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return await GetByUser(user.Id);
        }

        public async Task<IEnumerable<Notificacion>> GetByUser(string Id)
        {
            return await repositoryNotificacion.QueryAll()
                                           .Where(n => n.UserId == Id)
                                           .OrderByDescending(n => n.DateCreated)
                                           .ToListAsync();
        }

        public async Task<int> GetCountNotReadedByCurrentUser()
        {
            IdentityUser user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return (await GetCountNotReadedByUser(user.Id));
        }

        public async Task<int> GetCountNotReadedByUser(string Id)
        {
            return (await GetNotReadedByUser(Id)).Count();
        }

        public async Task<IEnumerable<Notificacion>> GetNotReadedByCurrentUser()
        {
            IdentityUser user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return await GetNotReadedByUser(user.Id);
        }

        public async Task<IEnumerable<Notificacion>> GetNotReadedByUser(string Id)
        {
            IEnumerable<Notificacion> list = await repositoryNotificacion.QueryAll()
                                           .Where(n => n.UserId == Id && n.Readed == false)
                                           .OrderByDescending(n => n.DateCreated)
                                           .ToListAsync();
            return list;
        }

        public async Task SetReadedByUser(string Id)
        {
            IEnumerable<Notificacion>list = await repositoryNotificacion.QueryAll()
                                           .Where(n => n.UserId == Id && n.Readed == false)
                                           .OrderByDescending(n => n.DateCreated)
                                           .ToListAsync();

            foreach(Notificacion item in list)
            {
                item.Readed = true;
                await repositoryNotificacion.UpdateAsync(item, item.Id);
            }
            await repositoryNotificacion.SaveChangesAsync();
        }
    }
}
