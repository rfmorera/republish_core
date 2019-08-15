using Microsoft.AspNetCore.Identity;
using Services.DTOs;
using Services.DTOs.Registro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserControlService
    {
        Task<IdentityResult> AddClient(IdentityUser user);
        Task<IdentityResult> AddAdmin(IdentityUser user);

        Task<IdentityResult> RemoveUser(string Id);

        Task<IdentityResult> DisableUser(IdentityUser user, bool flag);

        Task<IEnumerable<UserDTO>> GetClientList();

        Task<ClientDashboard> GetDashboard(IdentityUser user);
    }
}
