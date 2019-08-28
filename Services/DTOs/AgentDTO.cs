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

        public AgentDTO(string id, string userName, string phoneNumber)
        {
            Id = id;
            UserName = userName;
            Phone = phoneNumber;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
    }
}
