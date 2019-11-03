using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface INotificationsService
    {
        Task<IEnumerable<Notificacion>> GetByUser(string Id);
        Task SetReadedByUser(string Id);
    }
}
