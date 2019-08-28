using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class AgentDTO
    {
        public AgentDTO(string UserName, string Phone)
        {
            this.UserName = UserName;
            this.Phone = Phone;
        }

        public AgentDTO(IdentityUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Phone = user.PhoneNumber;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
    }
}
