using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class AgentDetailsDTO : AgentDTO
    {
        public AgentDetailsDTO(IdentityUser user, double current, double last) : base(user)
        {
            SalesCurrentMonth = current;
            SalesLastMonth = last;
        }

        public double SalesCurrentMonth { get; set; }
        public double SalesLastMonth { get; set; }
    }
}
