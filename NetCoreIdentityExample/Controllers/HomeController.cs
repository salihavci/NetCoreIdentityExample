using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreIdentityExample.DTO.ViewModels;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NetCoreIdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }
        public HomeController(ILogger<HomeController> logger,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserVM users)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = new AppUser();
                    appUser.UserName = users.UserName;
                    appUser.Email = users.Email;
                    appUser.PhoneNumber = users.PhoneNumber;
                    IdentityResult result = await _userManager.CreateAsync(appUser,users.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                    
                }
                return View(users);
            }
            catch (Exception ex)
            {
                return View(users);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser users = await _userManager.FindByEmailAsync(user.Email);
                    if (users != null)
                    {
                        if (await _userManager.IsLockedOutAsync(users)) //Kullanıcı kilitlenme durumunu kontrol et.
                        {
                            ModelState.AddModelError("","Hesabınız bir süreliğine kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                        }

                        await _signInManager.SignOutAsync();
                        //3. parametre beni hatırla bölümü için
                        //4. parametre kullanıcı kilitleme için
                        SignInResult result = await _signInManager.PasswordSignInAsync(users, user.Password, user.RememberMe, false);
                        if (result.Succeeded)
                        {
                            await _userManager.ResetAccessFailedCountAsync(users); // Başarısız giriş sayısını sıfırlar.
                            if (TempData["ReturnUrl"] != null)
                            {
                                return Redirect(TempData["ReturnUrl"].ToString());
                            }
                            return RedirectToAction("Index", "Member");
                        }
                        //else if (result.IsLockedOut) // Kullanıcı kilitli mi
                        //{
                        //    ModelState.AddModelError("", "Hesabınıza 3 kez başarısız giriş yaptınız. 20 dakika süre ile hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                        //}
                        else
                        {
                            if (!result.IsLockedOut)
                            {
                                await _userManager.AccessFailedAsync(users);
                                int failedCount = await _userManager.GetAccessFailedCountAsync(users);
                                ModelState.AddModelError("", $"{failedCount} kez başarısız giriş sağladınız. Lütfen tekrar deneyiniz.");
                                if (failedCount == 3)
                                {
                                    await _userManager.SetLockoutEndDateAsync(users, System.DateTimeOffset.Now.AddMinutes(20));
                                    ModelState.AddModelError("", "Hesabınıza 3 kez başarısız giriş yaptınız. 20 dakika süre ile hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Geçersiz email adresi ve ya şifresi.");
                                }
                            }
                            
                        }
                        
                        //else if (result.IsNotAllowed) // Kullanıcı giriş yapıp yapamayacağı durumda doğru bilgileri girdiğindeki durum
                        //{ 

                        //}
                    }
                    else
                    {
                        //ModelState.AddModelError(nameof(LoginVM.Email),"Geçersiz email adresi ve ya şifresi."); // Sadece Email'in summary kısmına modelerror ekler
                        ModelState.AddModelError("","Bu Email adresine ait kayıtlı kullanıcı bulunamamıştır.");
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
