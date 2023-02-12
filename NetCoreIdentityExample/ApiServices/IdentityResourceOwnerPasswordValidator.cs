using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using NetCoreIdentityExample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.ApiServices
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<AppUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var existUser = await _userManager.FindByEmailAsync(context.UserName).ConfigureAwait(false);
            if (existUser == null) {
                var errors = new Dictionary<string, object>
                {
                    { "errors", new List<string>() { "Email ya da şifreniz yanlış." } }
                };
                context.Result.CustomResponse = errors;
                return;
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(existUser,context.Password).ConfigureAwait(false);
            if (!passwordCheck)
            {
                var errors = new Dictionary<string, object>
                {
                    { "errors", new List<string>() { "Email ya da şifreniz yanlış." } }
                };
                context.Result.CustomResponse = errors;
                return;
            }
            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
