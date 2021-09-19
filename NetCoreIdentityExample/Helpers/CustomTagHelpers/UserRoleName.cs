using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.CustomTagHelpers
{
    [HtmlTargetElement("td",Attributes ="user-roles")]
    public class UserRoleName:TagHelper
    {
        public UserManager<AppUser> _userManager { get; set; }
        public UserRoleName(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser user = await _userManager.FindByIdAsync(UserId);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            string html = string.Empty;
            roles.ToList().ForEach(x=> {
                html += $"<span class=\"badge badge-info\" style=\"margin-left:.2vw;\">{x}</span>";
            });
            output.Content.SetHtmlContent(html);
        }
    }
}
