using Microsoft.AspNetCore.Identity;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.Validations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            try
            {
                List<IdentityError> errorList = new List<IdentityError>();
                if (password.ToLower().Contains(user.UserName.ToLower()))
                {
                    if (!user.Email.Contains(user.UserName))
                    {
                        errorList.Add(new IdentityError() { Code = "PasswordContainsUserName", Description = "Şifreniz kullanıcı adınızı içeremez." });
                    }
                }
                if (password.ToLower().Contains("1234")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains1234", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains("2345")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains2345", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains("3456")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains3456", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains("4567")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains4567", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains("5678")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains5678", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains("6789")) //1234 olursa
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContains6789", Description = "Şifreniz ardışık sayı içeremez." });
                }
                if (password.ToLower().Contains(user.Email.ToLower()))
                {
                    errorList.Add(new IdentityError() { Code = "PasswordContainsEmail", Description = "Şifreniz Email adresinizi içeremez." });
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
