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
using System.Security.Claims;
using NetCoreIdentityExample.Service;

namespace NetCoreIdentityExample.Controllers
{
    [Authorize(Roles = "Admin,Members")] //Bu controller'a sadece üyeler girebilir.
    //[Authorize] //Bu controller'a sadece üyeler girebilir.
    public class MemberController : BaseController
    {
        private readonly TwoFactorService _twoFactorService;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TwoFactorService twoFactorService) : base(null, userManager, signInManager, null, null)
        {
            _twoFactorService = twoFactorService;
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
        public async Task<IActionResult> UserEdit(UserVM user, IFormFile userPicture)
        {
            try
            {
                ModelState.Remove("Password");//Password alanını kontrol etmemesi için modelden çıkardık.
                ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
                if (ModelState.IsValid)
                {

                    AppUser users = await CurrentUser;
                    string phone = _userManager.GetPhoneNumberAsync(users).Result;
                    if (phone != user.PhoneNumber)
                    {
                        if (_userManager.Users.Any(m => m.PhoneNumber == user.PhoneNumber))
                        {
                            ModelState.AddModelError("", "Bu telefon numarası sistemde kayıtlıdır. Lütfen tekrar deneyiniz.");
                            return View(user);
                        }
                    }
                    if (users != null)
                    {
                        users.UserName = user.UserName;
                        users.Email = user.Email;
                        users.PhoneNumber = user.PhoneNumber;
                        if (userPicture != null && userPicture.Length > 0)
                        {
                            var filename = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
                            using (var stream = new FileStream(path, FileMode.Create))
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

        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (ReturnUrl.Contains("ViolancePage"))
            {
                ViewBag.message = "Erişmeye çalıştığınız sayfa şiddet içerikli videolar içerdiğinden dolayı 15 yaşından büyük olmanız gerekmektedir.";
            }
            else if (ReturnUrl.Contains("BursaPage"))
            {
                ViewBag.message = "Bu sayfaya sadece şehir alanı Bursa olan kullanıcılar erişebilir.";
            }
            else if (ReturnUrl.Contains("Exchange"))
            {
                ViewBag.message = "30 günlük kullanım süreniz dolmuştur. Lütfen özellikleri tekrar kullanmak için ücretli paketlerimizi satın alınız.";
            }
            else
            {
                ViewBag.message = "Bu sayfaya erişim izniniz bulunmamaktadır. Erişim izni almak için site yöneticisi ile görüşünüz.";
            }
            return View();
        }

        [Authorize(Roles = "Editor")] //Rol bazlı yetki kontrolü
        [HttpGet]
        public IActionResult Editor()
        {
            return View();
        }

        [Authorize(Policy = "BursaPolicy")] //Claim Bazlı yetki kontrolü
        [HttpGet]
        public IActionResult BursaPage()
        {
            return View();
        }


        [Authorize(Policy = "ViolancePolicy")]
        [HttpGet]
        public IActionResult ViolancePage()
        {
            return View();
        }

        public async Task<IActionResult> ExchangeRedirect()
        {
            bool result = User.HasClaim(x => x.Type == "ExpireDateExchange");
            if (!result)
            {
                Claim ExchangeClaim = new Claim("ExpireDateExchange", DateTime.Now.AddDays(30).Date.ToShortDateString(), ClaimValueTypes.String, "Internal");
                await _userManager.AddClaimAsync(CurrentUser.Result, ExchangeClaim);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(CurrentUser.Result, true);
            }
            return RedirectToAction("Exchange", "Member");
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult Exchange()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorWithAuthenticator()
        {
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser.Result);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(CurrentUser.Result);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser.Result);
            }
            var vm = new AuthenticatorVM();
            vm.SharedKey = unformattedKey;
            vm.AuthenticatorURI = _twoFactorService.GenerateQrCodeUri(CurrentUser.Result.Email, unformattedKey);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticatorVM vm)
        {
            var user = CurrentUser.ConfigureAwait(false).GetAwaiter().GetResult();
            var verificationCode = vm.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2FATokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (is2FATokenValid)
            {
                user.TwoFactorEnabled = true;
                user.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;
                //await _userManager.UpdateAsync(user);
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);
                TempData["recoveryCodes"] = recoveryCodes;
                TempData["message"] = "İki adımlı kimlik doğrulama tipiniz Microsoft/Google Authenticator olarak belirlenmiştir.";
                //Buradaki Async işleminden dönen ID tracking hatasına bakılacak 
                return RedirectToAction("TwoFactorAuth", "Member");
            }
            else
            {
                ModelState.AddModelError("", "Girdiğiniz doğrulama kodu yanlıştır. Lütfen tekrar deneyiniz.");
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult TwoFactorAuth()
        {
            return View(new AuthenticatorVM() { TwoFactorType = (TwoFactor)CurrentUser.Result.TwoFactor });
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticatorVM vm)
        {
            var user = CurrentUser.ConfigureAwait(false).GetAwaiter().GetResult();

            switch (vm.TwoFactorType)
            {
                case TwoFactor.None:
                    user.TwoFactorEnabled = false;
                    user.TwoFactor = (sbyte)TwoFactor.None;
                    TempData["message"] = "İki adımlı kimlik doğrulama tipiniz hiçbiri olarak belirlenmiştir.";
                    break;
                case TwoFactor.Phone:
                    if (string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        ViewBag.warning = "Telefon numaranız belirtilmemiştir. Lütfen kullanıcı güncelleme sayfasından telefon numaranızı belirtiniz.";
                    }
                    user.TwoFactorEnabled = true;
                    user.TwoFactor = (sbyte)TwoFactor.Phone;
                    TempData["message"] = "İki adımlı kimlik doğrulama tipiniz telefon onayı olarak belirlenmiştir.";
                    break;
                case TwoFactor.Email:
                    user.TwoFactorEnabled = true;
                    user.TwoFactor = (sbyte)TwoFactor.Email;
                    TempData["message"] = "İki adımlı kimlik doğrulama tipiniz email onayı olarak belirlenmiştir.";
                    break;
                case TwoFactor.MicrosoftGoogle:
                    return RedirectToAction("TwoFactorWithAuthenticator", "Member");
                default:
                    break;
            }
            await _userManager.UpdateAsync(user);
            return View(vm);

        }



    }
}
