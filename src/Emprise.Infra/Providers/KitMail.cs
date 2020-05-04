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
        public KitMail(IOptionsMonitor<AppConfig> appConfig)
        {
            _appConfig = appConfig.CurrentValue;
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

                client.Connect(_appConfig.Email.SmtpServer, _appConfig.Email.SmtpPort, false);


                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_appConfig.Email.AccountName, _appConfig.Email.Password /*"AZ8UYdAA9NxPCvKxbEfX"*/);

                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }


    }
}
