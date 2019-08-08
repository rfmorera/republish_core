using Microsoft.AspNetCore.Identity;
using Models;
using Republish.Data;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services.Impls
{
    public class UserControlService : IUserControlService
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        public UserControlService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddAdmin(IdentityUser user)
        {
            IdentityResult result = await _userManager.CreateAsync(user, "Servicios*2019");
            if (!result.Succeeded)
            {
                return result;
            }
            result = await _userManager.AddToRoleAsync(user, RTRoles.Admin);
            return result;
        }

        public async Task<IdentityResult> AddClient(IdentityUser user)
        {
            IdentityResult result = await _userManager.CreateAsync(user, "Servicios*2019");
            if (!result.Succeeded)
            {
                return result;
            }
            result = await _userManager.AddToRoleAsync(user, RTRoles.Client);
            return result;
        }

        public async Task<IdentityResult> DisableUser(IdentityUser user, bool flag)
        {
            return await _userManager.SetLockoutEnabledAsync(user, flag);
        }

        public async Task<IdentityResult> RemoveUser(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<UserDTO>> GetClientList()
        {
            IEnumerable<UserDTO> list = (await _userManager.GetUsersInRoleAsync(RTRoles.Client))
                                                .Select(t => new UserDTO(t.Id, t.UserName, t.Email, RTRoles.Client));

            return list;
        }
    }
}
