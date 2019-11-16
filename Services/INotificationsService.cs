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
        Task<IEnumerable<Notificacion>> GetNotReadedByUser(string Id);
        Task<int> GetCountNotReadedByUser(string Id);
        Task<IEnumerable<Notificacion>> GetByCurrentUser();
        Task<IEnumerable<Notificacion>> GetNotReadedByCurrentUser();
        Task<int> GetCountNotReadedByCurrentUser();
        Task SetReadedByUser(string Id);
        Task Add(Notificacion notificacion);
        Task Add(IEnumerable<Notificacion> notificacions);
        Task SendNotification(string UserId, string message);
    }
}
