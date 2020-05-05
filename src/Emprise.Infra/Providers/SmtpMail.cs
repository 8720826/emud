using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Providers
{
    public class SmtpMail : IMail
    {
        private readonly ILogger<SmtpMail> _logger;
        private readonly AppConfig _appConfig;
        public SmtpMail(IOptions<AppConfig> appConfig, ILogger<SmtpMail> logger)
        {
            _appConfig = appConfig.Value;
            _logger = logger;
        }


        public async Task Send(MailModel message)
        {
            await Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(_appConfig.Email.SmtpServer, _appConfig.Email.SmtpPort);
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_appConfig.Email.AccountName, _appConfig.Email.Password);
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_appConfig.Email.AccountName, _appConfig.Email.FromAlias);
                    mailMessage.To.Add(message.Address);
                    mailMessage.Body = message.Content;
                    mailMessage.Subject = message.Subject;
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendEmail Exception");
                    throw ex;
                }

            }).ConfigureAwait(false);
        }


    }
}
