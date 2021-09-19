using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected UserManager<AppUser> _userManager { get; }
        protected SignInManager<AppUser> _signInManager { get; }
        protected readonly IConfiguration _config;
        protected Task<AppUser> CurrentUser => _userManager.FindByNameAsync(User.Identity.Name);
        protected RoleManager<AppRole> _roleManager { get; }

        public BaseController(ILogger<BaseController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration config,RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
        }
        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
    }
}
