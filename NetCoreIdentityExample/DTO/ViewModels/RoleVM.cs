using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class RoleVM
    {
        [Required(ErrorMessage ="Rol adı gereklidir.")]
        [Display(Name="Rol adı")]
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
