using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class ChangePasswordVM
    {
        [Display(Name ="Eski şifreniz")]
        [Required(ErrorMessage ="Eski şifrenizi girmeniz gerekmektedir.")]
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage ="Şifreniz en az 8 karakterli olmalıdır.")]
        public string PasswordOld { get; set; }

        [Display(Name = "Yeni şifreniz")]
        [Required(ErrorMessage = "Yeni şifrenizi girmeniz gerekmektedir.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Şifreniz en az 8 karakterli olmalıdır.")]
        public string PasswordNew { get; set; }

        [Display(Name = "Yeni şifrenizin tekrarı")]
        [Required(ErrorMessage = "Yeni şifrenizin tekrarını girmeniz gerekmektedir.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Şifreniz en az 8 karakterli olmalıdır.")]
        [Compare("PasswordNew",ErrorMessage ="Şifreniz ve şifrenizin tekrarı aynı olmak zorunda.")]
        public string PasswordConfirm { get; set; }
    }
}
