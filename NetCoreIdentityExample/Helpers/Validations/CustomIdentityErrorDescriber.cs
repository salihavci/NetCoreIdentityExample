using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.Validations
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "InvalidUserName",
                Description = $"Bu {userName} geçersizdir."
            };
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description = $"{email} adlı email adresi kullanılmaktadır."
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {

            return new IdentityError()
            {
                Code = "DuplicateUsername",
                Description = $"{userName} adlı kullanıcı adı kullanılmaktadır."
            };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code= "PasswordTooShort",
                Description = $"Şifreniz {length} karakterden az olamaz."
            };
        }

    }
}
