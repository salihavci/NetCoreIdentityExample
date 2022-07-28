using NetCoreIdentityExample.DTO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class AuthenticatorVM
    {
        public string SharedKey { get; set; }
        public string AuthenticatorURI { get; set; }
        [Display(Name="Doğrulama kodu")]
        [Required(ErrorMessage = "Doğrulama kodu gereklidir.")]
        public string VerificationCode { get; set; }
        [Display(Name = "İki adımlı kimlik doğrulama tipi")]
        public TwoFactor TwoFactorType { get; set; }
    }
}
