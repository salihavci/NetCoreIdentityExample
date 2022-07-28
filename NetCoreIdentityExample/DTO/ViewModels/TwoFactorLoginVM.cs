using NetCoreIdentityExample.DTO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class TwoFactorLoginVM
    {
        
        [Display(Name = "Doğrulama Kodunuz")]
        [Required(ErrorMessage = "Lütfen doğrulama kodunuzu giriniz.")]
        [StringLength(8,ErrorMessage = "Doğrulama kodunuz en fazla 8 karakterli olmalıdır.")]
        public string VerificationCode { get; set; }
        public bool isRememberMe { get; set; }
        public bool isRecoverCode { get; set; }
        public TwoFactor TwoFactorType { get; set; }

    }
}
