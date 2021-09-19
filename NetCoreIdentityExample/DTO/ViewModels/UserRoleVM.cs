using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.DTO.ViewModels
{
    public class UserRoleVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsChecked { get; set; }

    }
}
