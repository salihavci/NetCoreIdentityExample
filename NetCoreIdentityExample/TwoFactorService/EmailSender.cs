using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Service
{
    public class EmailSender
    {
        private readonly TwoFactorOptions _options;
        private readonly TwoFactorService _service;
        private readonly IConfiguration _configuration;

        public EmailSender(IOptions<TwoFactorOptions> options, TwoFactorService service,IConfiguration configuration)
        {
            _options = options.Value;
            _service = service;
            _configuration = configuration;
        }

        public string Send(string emailAddress)
        {
            string code = _service.GetCodeVerification().ToString();
            Execute(emailAddress, code).Wait();
            return code;
        }

        public async Task Execute(string email, string code)
        {
            var client = new SendGridClient(_options.SendGrid_ApiKey.ToString());
            var from = new EmailAddress(_configuration["MailSender"].ToString()); //Burada kendi mail adresimi çekmeye çalışıyorum (Gmail Desteği Yok)
            var subject = "İki Adımlı doğrulama kodunuz";
            var to = new EmailAddress(email);
            var htmlContent = $"<h2>Siteye giriş yapabilmek için doğrulama kodunuz aşağıdadır.</h2><h3>Kodunuz : {code}</h3>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
