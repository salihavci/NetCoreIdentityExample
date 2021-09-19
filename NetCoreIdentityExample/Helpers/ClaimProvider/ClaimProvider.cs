using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.ClaimProvider
{
    public class ClaimProvider : IClaimsTransformation
    {

        private UserManager<AppUser> _userManager { get; set; }
        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) //Manual Claim eklemek için gerekli servis fonksiyonu
        {
            try
            {
                if (principal != null && principal.Identity.IsAuthenticated) //Kullanıcı kayıtlı mı ve claims boş değil mi kontrol et
                {
                    ClaimsIdentity claims = principal.Identity as ClaimsIdentity; //Type casting
                    AppUser user = await _userManager.FindByNameAsync(claims.Name);
                    if (user != null)
                    {
                        if (user.City != null)
                        {
                            if (!principal.HasClaim(c => c.Type == "City")) // Eğer City isminde bir claim yoksa ekle demek
                            {
                                Claim cityClaim = new Claim("City",user.City,ClaimValueTypes.String,"Internal");
                                claims.AddClaim(cityClaim);
                            }
                        }
                        if (user.Birthday != null)
                        {
                            var today = DateTime.Today;
                            var age = today.Year - user.Birthday?.Year;
                            if (age > 15)
                            {
                                Claim ageClaim = new Claim("Violance", true.ToString(), ClaimValueTypes.String, "Internal");
                                claims.AddClaim(ageClaim);
                            }
                        }
                    }
                }
                return principal;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
