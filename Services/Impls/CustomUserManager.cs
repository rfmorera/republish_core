using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Services.Impls
{
    public class CustomUserManager : UserManager<IdentityUser>
    {
        public CustomUserManager(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators, IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<IdentityUser>> logger)
            :base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public async override Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)
        {
            IdentityResult result = await base.ResetPasswordAsync(user, token, newPassword);

            if (result == IdentityResult.Success)
            {
                // If a user is locked out then they must reset their password before their account is unlocked. So reset the lock out date for this user.
                await SetLockoutEndDateAsync(user, null);
            }

            return result;
        }
    }
}
