using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class ResetPasswordVM
    {
        [Display(Name ="Email Adresiniz")]
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress()]
        public string Email { get; set; }

        [Display(Name = "Yeni Parolanız")]
        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Parolanızın uzunluğu en az 8 karakterli olmalıdır.")]
        public string PasswordNew { get; set; }

    }
}
