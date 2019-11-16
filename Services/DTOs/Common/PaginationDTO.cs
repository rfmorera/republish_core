using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Common
{
    public class PaginationDTO
    {
        public PaginationDTO(Pager pager, string controller, string action)
        {
            Pager = pager;
            Controller = controller;
            Action = action;
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public Pager Pager { get; set; }
    }
}
