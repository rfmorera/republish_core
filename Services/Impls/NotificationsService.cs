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

namespace Services.Impls
{
    public class NotificationsService : INotificationsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Notificacion> repositoryNotificacion;

        public NotificationsService(ApplicationDbContext context)
        {
            _context = context;
            repositoryNotificacion = new Repository<Notificacion>(context);
        }

        public async Task<IEnumerable<Notificacion>> GetByUser(string Id)
        {
            return await repositoryNotificacion.QueryAll()
                                           .Where(n => n.UserId == Id)
                                           .OrderByDescending(n => n.DateCreated)
                                           .ToListAsync();
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
