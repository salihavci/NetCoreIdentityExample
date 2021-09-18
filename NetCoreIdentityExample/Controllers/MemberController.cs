using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityExample.DTO.ViewModels;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
namespace NetCoreIdentityExample.Controllers
{
    [Authorize] //Bu controller'a sadece üyeler girebilir.
    public class MemberController : Controller
    {

        public UserManager<AppUser> _userManager { get; }
        public SignInManager<AppUser> _signInManager { get; }
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //[Authorize] //Bu sayfaya sadece üyeler girebilir.
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name.ToString());
            UserVM users = user.Adapt<UserVM>(); //Mapster kütüphanesi ile mapleme işlemi
            return View(users);
        }
    }
}
