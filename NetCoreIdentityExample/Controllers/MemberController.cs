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
using NetCoreIdentityExample.DTO.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace NetCoreIdentityExample.Controllers
{
    [Authorize(Roles ="Admin,Members")] //Bu controller'a sadece üyeler girebilir.
    public class MemberController : BaseController
    {

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager):base(null,userManager,signInManager,null,null)
        {
        }
        //[Authorize] //Bu sayfaya sadece üyeler girebilir.
        public async Task<IActionResult> Index()
        {
            AppUser user = await CurrentUser;
            UserVM users = user.Adapt<UserVM>(); //Mapster kütüphanesi ile mapleme işlemi
            return View(users);
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(ChangePasswordVM changed)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser user = await CurrentUser;
                    if (user != null)
                    {
                        bool exist = await _userManager.CheckPasswordAsync(user, changed.PasswordOld);
                        if (exist)
                        {
                            IdentityResult result = await _userManager.ChangePasswordAsync(user, changed.PasswordOld, changed.PasswordNew);
                            if (result.Succeeded)
                            {
                                await _userManager.UpdateSecurityStampAsync(user); //Şifre değiştiği için SecurityStamp'in değişmesi gerekmektedir.
                                await _signInManager.SignOutAsync();
                                await _signInManager.PasswordSignInAsync(user, changed.PasswordNew, true, false);
                                ViewBag.status = "Success";

                                //return RedirectToAction("Index", "Member");
                            }
                            else
                            {
                                AddModelError(result);

                                ViewBag.status = "Error";
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Eski şifreniz yanlış. Lütfen tekrar deneyiniz.");
                        }
                    }
                }
                return View(changed);
            }
            catch (Exception ex)
            {
                return View(changed);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            AppUser user = await CurrentUser;
            UserVM users = user.Adapt<UserVM>();
            ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(users);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserVM user,IFormFile userPicture)
        {
            try
            {
                ModelState.Remove("Password");//Password alanını kontrol etmemesi için modelden çıkardık.
                ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
                if (ModelState.IsValid)
                {
                    AppUser users = await  CurrentUser;
                    if (users != null)
                    {
                        users.UserName = user.UserName;
                        users.Email = user.Email;
                        users.PhoneNumber = user.PhoneNumber;
                        if (userPicture != null && userPicture.Length > 0)
                        {
                            var filename = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
                            using (var stream = new FileStream(path,FileMode.Create))
                            {
                                await userPicture.CopyToAsync(stream);
                                users.Picture = "/img/" + filename;
                            }
                        }
                        users.City = user.City;
                        users.Birthday = user.Birthday;
                        users.Gender = (int)user.Gender; //Enum to int type casting

                        IdentityResult result = await _userManager.UpdateAsync(users);
                        if (result.Succeeded)
                        {
                            await _userManager.UpdateSecurityStampAsync(users);
                            await _signInManager.SignOutAsync();
                            await _signInManager.SignInAsync(users, true);
                            ViewBag.status = "Success";
                        }
                        else
                        {
                            AddModelError(result);

                            ViewBag.status = "Error";
                        }
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {

            _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "Editor")]
        [HttpGet]
        public IActionResult Editor()
        {
            return View();
        }


    }
}
