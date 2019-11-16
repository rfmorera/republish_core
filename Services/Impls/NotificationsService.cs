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
using Republish.Extensions;
using Services.DTOs.Notification;
using Services.DTOs;
using Services.DTOs.Common;
using PagedList;

namespace Services.Impls
{
    public class NotificationsService : INotificationsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Notificacion> repositoryNotificacion;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        private const int pageSize = 10;

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

        public async Task<IndexDTO> GetByUser(string Id, int? page)
        {
            IEnumerable<Notificacion> userNotificacions = (await GetByUser(Id));
            int count = userNotificacions.Count();
            Pager pager = new Pager(count, page, pageSize);
            IEnumerable<NotificacionDTO> notificacions = userNotificacions.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).Select(a => new NotificacionDTO(a));
            return new IndexDTO(pager, notificacions);
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

        public async Task Add(Notificacion notificacion)
        {
            await repositoryNotificacion.AddAsync(notificacion);
            await repositoryNotificacion.SaveChangesAsync();
        }

        public async Task Add(IEnumerable<Notificacion> notificacions)
        {
            await repositoryNotificacion.AddAllAsync(notificacions);
            await repositoryNotificacion.SaveChangesAsync();
        }

        public async Task SendNotification(string UserId, string message)
        {
            Notificacion notificacion = new Notificacion()
            {
                UserId = UserId,
                Mensaje = message,
                DateCreated = DateTime.Now.ToUtcCuba(),
                Readed = false,
            };
            await Add(notificacion);
        }
    }
}
