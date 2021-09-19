using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreIdentityExample.DTO.ViewModels;
using NetCoreIdentityExample.Helpers;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NetCoreIdentityExample.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration config) : base(logger, userManager, signInManager, config, null)
        {

        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
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
                    if (_userManager.Users.Any(m => m.PhoneNumber == users.PhoneNumber))
                    {
                        ModelState.AddModelError("", "Bu telefon numarası sistemde kayıtlıdır. Lütfen tekrar deneyiniz.");
                        return View(users);
                    }
                    AppUser appUser = new AppUser();
                    appUser.UserName = users.UserName;
                    appUser.Email = users.Email;
                    appUser.PhoneNumber = users.PhoneNumber;
                    IdentityResult result = await _userManager.CreateAsync(appUser, users.Password);
                    if (result.Succeeded)
                    {
                        if (!_userManager.IsInRoleAsync(appUser, "Members").Result)
                        {
                            await _userManager.AddToRoleAsync(appUser, "Members");
                        }
                        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                        string link = Url.Action("ConfirmEmail", "Home", new { userId = appUser.Id, token = confirmationToken }, HttpContext.Request.Scheme);
                        EmailConfirmation confirmation = new EmailConfirmation(_config);
                        confirmation.SendMail(link, appUser.Email);
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        AddModelError(result);
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
                            ModelState.AddModelError("", "Hesabınız bir süreliğine kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                            return View(user);
                        }
                        if (!_userManager.IsEmailConfirmedAsync(users).Result)
                        {
                            ModelState.AddModelError("", "Hesabınız aktifleştirilemediği için giriş yapamazsınız. Öncelikle üyeliğinizi aktifleştirmeniz gerekmektedir.");
                            return View(user);
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
                        ModelState.AddModelError("", "Bu Email adresine ait kayıtlı kullanıcı bulunamamıştır.");
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
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([Bind("Email")] ResetPasswordVM user)
        {
            try
            {
                AppUser users = await _userManager.FindByEmailAsync(user.Email);
                PasswordReset reset = new PasswordReset(_config);
                if (users != null)
                {
                    string resetToken = await _userManager.GeneratePasswordResetTokenAsync(users);
                    string link = Url.Action("ResetPasswordConfirm", "Home", new { UserId = users.Id, token = resetToken }, HttpContext.Request.Scheme);
                    reset.SendMail(link, user.Email);
                    ViewBag.status = "Success";
                }
                else
                {
                    ModelState.AddModelError("", "Bu email adresine kayıtlı bir hesap bulunamamıştır. Lütfen tekrar deneyiniz.");
                    ViewBag.status = "Error";
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string UserId, string token)
        {
            TempData["UserId"] = UserId;
            TempData["token"] = token;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")] ResetPasswordVM user)
        {
            try
            {
                string token = TempData["token"].ToString();
                string UserId = TempData["UserId"].ToString();
                AppUser users = await _userManager.FindByIdAsync(UserId);
                if (users != null)
                {
                    IdentityResult result = await _userManager.ResetPasswordAsync(users, token, user.PasswordNew);
                    if (result.Succeeded)
                    {
                        //Şifre değiştikten sonraki snapshot'u alır ve o alanı günceller (SecurityStamp)
                        await _userManager.UpdateSecurityStampAsync(users);
                        ViewBag.status = "Success";
                        //return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddModelError(result);
                        return View(users);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Böyle bir kullanıcı bulunamadı. Lütfen tekrar deneyiniz.");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                TempData["UserId"] = userId;
                TempData["token"] = token;
                var user = await _userManager.FindByIdAsync(userId);
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    ViewBag.status = "Email adresiniz onaylanmıştır. Login ekranından kullanıcı girişi yapabilirsiniz.";
                }
                else
                {
                    ViewBag.status = "Bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz.";
                }
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.status = "Bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties); //Facebook'a kendi verdiğimiz property'ler ile git diyoruz.
        }

        [HttpGet]
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties); //Facebook'a kendi verdiğimiz property'ler ile git diyoruz.
        }

        [HttpGet]
        public IActionResult MicrosoftLogin(string ReturnUrl) {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
            return new ChallengeResult("Microsoft", properties); //Facebook'a kendi verdiğimiz property'ler ile git diyoruz.
        }

        [HttpGet]
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            try
            {
                ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                    if (result.Succeeded)
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        AppUser user = new AppUser();
                        user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value; //Kullanıcının emaili
                        string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value; //Kullanıcının facebook id'si
                        if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name)) //Kullanıcının Adı ve Soyadı
                        {
                            string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                            userName = userName.Replace(" ", "-").ToLower() + ExternalUserId.Substring(0, 5).ToString();
                            user.UserName = userName;
                        }
                        else
                        {
                            user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                        }
                        AppUser user2 = await _userManager.FindByEmailAsync(user.Email);
                        if (user2 == null)
                        {
                            user.EmailConfirmed = true;
                            IdentityResult results = await _userManager.CreateAsync(user);
                            if (results.Succeeded)
                            {
                                IdentityResult LoginResult = await _userManager.AddLoginAsync(user, info); //Facebook - Google login olduğunda tabloya kayıt atar - Eğer kayıtlı değilse.
                                if (LoginResult.Succeeded)
                                {
                                    //Normal kayıt - giriş için kullanılan method
                                    //await _signInManager.SignOutAsync();
                                    //await _signInManager.SignInAsync(user, true);

                                    //External login - signup için kullanılan method (Claim için yapıldı)
                                    if (!_userManager.IsInRoleAsync(user, "Members").Result)
                                    {
                                        await _userManager.AddToRoleAsync(user, "Members");
                                    }
                                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    AddModelError(LoginResult);
                                }
                            }
                            else
                            {
                                AddModelError(results);
                            }
                        }
                        else
                        {
                            IdentityResult LoginResult = await _userManager.AddLoginAsync(user2, info);
                            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                            return Redirect(ReturnUrl);
                        }
                    }
                }
                List<string> errors = ModelState.Values.SelectMany(x=> x.Errors).Select(y=> y.ErrorMessage).ToList();
                return View("Error", errors);
            }
            catch (Exception ex)
            {
                return View("Error", null);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
