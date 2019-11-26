using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Providers
{
    public class KitMail : IMail
    {
        private readonly AppConfig _appConfig;
        public KitMail(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
        }


        public async Task Send(MailModel message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_appConfig.Email.FromAlias, _appConfig.Email.AccountName));
            mimeMessage.To.Add(new MailboxAddress(message.Address, message.Address));

            mimeMessage.Subject = message.Subject;

            mimeMessage.Body = new TextPart("html") { Text = message.Content };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.qq.com", 465, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("27800734@qq.com", "6873336");

                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }


    }
}
