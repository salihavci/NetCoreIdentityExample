using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
namespace NetCoreIdentityExample.Helpers
{
    public class PasswordReset
    {
        private readonly IConfiguration _configuration;

        public PasswordReset(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(string link)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_configuration["MailSettings:Username"].ToString()));
                message.To.Add(MailboxAddress.Parse("salih@f1yazilim.com.tr"));
                message.Subject = $"Şifre sıfırlama isteği";
                message.Body = new TextPart(TextFormat.Html) { Text = $"<h2>Şifrenizi sıfırlamak için lütfen aşağıdaki linke tıklayınız.</h2><hr /><a href='{link}'>Şifre sıfırlama linki</a>" };

                using var client = new SmtpClient();
                client.Connect(_configuration["MailSettings:Host"], Int32.Parse(_configuration["MailSettings:Port"]), SecureSocketOptions.Auto);
                client.Authenticate(_configuration["MailSettings:Username"].ToString(), _configuration["MailSettings:Password"].ToString());
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {

            }
           
        }
    }
}
