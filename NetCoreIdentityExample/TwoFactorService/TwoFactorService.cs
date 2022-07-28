using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Service
{
    public class TwoFactorService
    {
        private readonly UrlEncoder _encoder;
        private readonly TwoFactorOptions _options;

        public TwoFactorService(UrlEncoder encoder,IOptions<TwoFactorOptions> options)
        {
            _encoder = encoder;
            _options = options.Value;
        }

        public int GetCodeVerification()
        {
            Random rnd = new Random();
            return rnd.Next(1000, 9999);
        }

        public int TimeLeft(HttpContext context)
        {
            if (context.Session.GetString("currentTime") == null)
            {
                context.Session.SetString("currentTime", DateTime.Now.AddSeconds(_options.CodeTimeExpire).ToString());
            }
            DateTime currentTime = DateTime.Parse(context.Session.GetString("currentTime").ToString());

            int timeLeft = (int)(currentTime - DateTime.Now).TotalSeconds;

            if (timeLeft <= 0)
            {
                context.Session.Remove("currentTime");
                return 0;
            }
            else
            {
                return timeLeft;
            }
        }

        public string GenerateQrCodeUri(string email, string unformattedKey)
        {
            const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(format, _encoder.Encode("www.salihavci.com"), _encoder.Encode(email), unformattedKey);
        }
    }
}
