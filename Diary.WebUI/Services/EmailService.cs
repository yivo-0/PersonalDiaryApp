using Diary.WebUI.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Diary.WebUI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _mailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task Send(Email email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Admin", email.EmailFrom));
            emailMessage.To.Add(new MailboxAddress("", email.EmailTo));
            emailMessage.Subject = "Invation token";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = email.Message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, false);
                await client.AuthenticateAsync(_mailSettings.EMail, _mailSettings.Password);
                try
                {
                    await client.SendAsync(emailMessage);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        System.Net.Mail.SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == System.Net.Mail.SmtpStatusCode.MailboxBusy || status == System.Net.Mail.SmtpStatusCode.MailboxUnavailable)
                        {
                            await client.SendAsync(emailMessage);
                        }
                    }
                }
                await client.DisconnectAsync(true);
            }

        }
    }
}
