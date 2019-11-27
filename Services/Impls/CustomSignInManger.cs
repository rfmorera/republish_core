using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Services.Impls
{
    public class ICustomSignInManger : SignInManager<IdentityUser>
    {
        public ICustomSignInManger(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<IdentityUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        private async Task<bool> IsTfaEnabled(IdentityUser user)
            => UserManager.SupportsUserTwoFactor &&
            await UserManager.GetTwoFactorEnabledAsync(user) &&
            (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        public Task SignInAsync(IdentityUser user)
        {
            return base.SignInAsync(user, false);
        }

        public async Task<SignInResult> SignInAsync(IdentityUser user, bool isPersistent)
        {
            string userName = user.UserName;
            user = await UserManager.FindByNameAsync(userName);
            SignInResult result;

            if (user != null)
            {
                if (UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user))
                {
                    result = await LockedOut(user);
                }
                else
                {
                    if (await IsTfaEnabled(user))
                    {
                        if (await IsTwoFactorClientRememberedAsync(user))
                        {
                            result = SignInResult.Success;
                        }
                        else
                        {
                            return await SignInOrTwoFactorAsync(user, isPersistent);
                        }
                    }
                    else
                    {
                        result = SignInResult.Success;
                    }
                }
            }
            else
            {
                result = SignInResult.Failed;
            }

            return result;
        }

        public async Task<SignInResult> VerifyTwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
        {
            var twoFactorInfo = await GetTwoFactorAuthenticationUserAsync();
            
            if (twoFactorInfo == null || twoFactorInfo.Id == null)
            {
                return SignInResult.Failed;
            }
            var user = await UserManager.FindByIdAsync(twoFactorInfo.Id.ToString());
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }

            if (await UserManager.VerifyTwoFactorTokenAsync(user, Options.Tokens.AuthenticatorTokenProvider, code))
            {
                await ResetLockout(user);
                return SignInResult.Success;
            }

            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user);
            if (await UserManager.IsLockedOutAsync(user))
            {
                return await LockedOut(user);
            }

            return SignInResult.Failed;
        }

        public async Task ResetLockoutOnSuccessfulResetPassword(IdentityUser user)
        {
            await ResetLockout(user);
        }
    }
}
