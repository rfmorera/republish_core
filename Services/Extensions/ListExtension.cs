using Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
{
    public static class ListExtension
    {
        public static IEnumerable<NotificacionDTO> ToNotificacionDTO(this IEnumerable<Notificacion> source)
        {
            List<NotificacionDTO> list = new List<NotificacionDTO>();
            foreach(Notificacion item in source)
            {
                list.Add(new NotificacionDTO(item));
            }
            return list;
        }
    }
}
