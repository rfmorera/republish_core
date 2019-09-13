using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class AgentDetailsDTO : AgentDTO
    {
        public AgentDetailsDTO(IdentityUser user, double current, double last, IEnumerable<RecargaDetail> recargas) : base(user)
        {
            SalesCurrentMonth = current;
            SalesLastMonth = last;
            Recargas = recargas;
        }

        public double SalesCurrentMonth { get; set; }
        public double SalesLastMonth { get; set; }

        public IEnumerable<RecargaDetail> Recargas { get; set; }
    }
}
