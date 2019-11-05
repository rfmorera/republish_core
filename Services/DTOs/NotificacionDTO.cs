using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class NotificacionDTO
    {
        public NotificacionDTO(Notificacion item)
        {
            Id = item.Id;
            Mensaje = item.Mensaje;
            Readed = item.Readed;
            DateCreated = item.DateCreated;
        }

        public string Id { get; set; }

        public string Mensaje { get; set; }
        public bool Readed { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status
        {
            get
            {
                if (Readed) return "alert-warning";
                return "alert-success";
            }
        }
    }
}
