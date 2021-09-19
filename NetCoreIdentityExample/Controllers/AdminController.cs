using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityExample.DTO.ViewModels;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager) : base(null, userManager, null, null,roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRole(RoleVM role)
        {
            try
            {
                AppRole roles = new AppRole();
                roles.Name = role.Name;
                IdentityResult result = _roleManager.CreateAsync(roles).Result; //Async yazmaktan kaçınmak için Result yazıldı.
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Admin");
                }
                else
                {
                    AddModelError(result);
                }
                return View(role);
            }
            catch (Exception ex)
            {
                return View(role);
            }
        }

        [HttpGet]
        public IActionResult DeleteRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteRole(string id)
        {
            try
            {
                AppRole role = _roleManager.FindByIdAsync(id).Result;
                if (role != null)
                {
                    IdentityResult result = _roleManager.DeleteAsync(role).Result;
                    //if (result.Succeeded)
                    //{
                    //    return RedirectToAction("Roles", "Admin");
                    //}
                    //else
                    //{
                    //    ViewBag.hata = "Hata meydana geldi";
                    //    //AddModelError(result);
                    //}
                }
                //else
                //{
                //    ViewBag.hata = "Veritabanında böyle bir rol ismi bulunamamıştır.";
                //}
                return RedirectToAction("Roles", "Admin");
            }
            catch (Exception ex)
            {
                //return View();
                return RedirectToAction("Roles", "Admin");
            }
        }

        [HttpGet]
        public IActionResult UpdateRole(string id)
        {
            try
            {
                AppRole role = _roleManager.FindByIdAsync(id).Result;
                if (role != null)
                {
                    return View(role.Adapt<RoleVM>());
                }
                else
                {
                    return RedirectToAction("Roles", "Admin");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Roles", "Admin");
            }
        }

        [HttpPost]
        public IActionResult UpdateRole(RoleVM role)
        {
            try
            {
                AppRole roles = _roleManager.FindByIdAsync(role.Id).Result;
                if (roles != null)
                {
                    roles.Name = role.Name;
                    IdentityResult result = _roleManager.UpdateAsync(roles).Result;
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles", "Admin");
                    }
                    else
                    {
                        AddModelError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu.");
                }
                return View(role);
            }
            catch (Exception ex)
            {
                return View(role);
            }
        }

        [HttpGet]
        public IActionResult AssignRole(string id)
        {
            AppUser user = _userManager.FindByIdAsync(id).Result;
            TempData["userId"] = user.Id;
            if (user != null)
            {
                ViewBag.username = user.UserName;
                IQueryable<AppRole> roles = _roleManager.Roles;
                List<string> role = _userManager.GetRolesAsync(user).Result.ToList();
                List<UserRoleVM> userRole = new List<UserRoleVM>();
                foreach (var item in roles)
                {
                    UserRoleVM r = new UserRoleVM();
                    r.RoleId = item.Id;
                    r.RoleName = item.Name;
                    if (role.Contains(item.Name))
                    {
                        r.IsChecked = true;
                    }
                    else
                    {
                        r.IsChecked = false;
                    }
                    userRole.Add(r);
                }
                return View(userRole);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(List<UserRoleVM> roles)
        {
            try
            {
                AppUser user = _userManager.FindByIdAsync(TempData["userId"].ToString()).Result;
                foreach (var item in roles)
                {
                    if (item.IsChecked)
                    {
                        await _userManager.AddToRoleAsync(user, item.RoleName);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                    }
                }
                return RedirectToAction("Users", "Admin");
            }
            catch (Exception ex)
            {
                return View(roles);
            }
        }

        [HttpGet]
        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        [HttpGet]
        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }
    }
}
