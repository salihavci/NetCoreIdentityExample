using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers
{
    public class EmailConfirmation
    {
        private readonly IConfiguration _configuration;

        public EmailConfirmation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(string link, string email)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_configuration["MailSettings:Username"].ToString()));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = $"Email Doğrulama mesajı";
                message.Body = new TextPart(TextFormat.Html) { Text = $"<h2>Epostanızı doğrulamak için lütfen aşağıdaki linke tıklayınız.</h2><hr /><a href='{link}'>Eposta doğrulama linki</a>" };

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
