
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class UserDTO
    {
        public UserDTO(string Id, string userName, string email, string Role, double saldo)
        {
            this.Id = Id;
            Username = userName;
            Email = email;
            this.Role = Role;
            SaldoActual = Math.Round(saldo, 3);
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public double SaldoActual { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
