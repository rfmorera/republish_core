using Models;
using Services.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Notification
{
    public class IndexDTO
    {
        public IndexDTO(Pager pager, IEnumerable<NotificacionDTO> notificacions)
        {
            Pager = pager;
            Notificacions = notificacions;
        }

        public IEnumerable<NotificacionDTO> Notificacions { get; set; }
        public Pager Pager { get; set; }
    }
}
