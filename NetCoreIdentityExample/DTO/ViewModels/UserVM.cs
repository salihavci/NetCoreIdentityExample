﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class UserVM
    {
        [Required(ErrorMessage ="Kullanıcı ismi gereklidir.")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [Display(Name = "Email Adresi")]
        [EmailAddress(ErrorMessage ="Email adresiniz doğru formatta değildir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
