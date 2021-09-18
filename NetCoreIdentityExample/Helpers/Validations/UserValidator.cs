using Microsoft.AspNetCore.Identity;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.Validations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            try
            {
                List<IdentityError> errorList = new List<IdentityError>();
                string[] Digits = new string[]{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                foreach (var item in Digits)
                {
                    if (user.UserName[0].ToString().Equals(item)) {
                        errorList.Add(new IdentityError() { Code = "UsernameContainsFirstLetterDigits", Description = "Kullanıcı adınızın ilk karakteri sayısal değer içeremez." });
                    }
                }
                if (errorList.Count == 0)
                {
                    return Task.FromResult(IdentityResult.Success);
                }
                else
                {
                    return Task.FromResult(IdentityResult.Failed(errorList.ToArray()));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
