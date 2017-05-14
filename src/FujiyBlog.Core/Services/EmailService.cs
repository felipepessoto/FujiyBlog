using FujiyBlog.Core.EntityFramework;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    [Obsolete("Use MessageServices class")]
    public class EmailService
    {
        private readonly SettingRepository settingRepository;

        public EmailService(SettingRepository settingRepository)
        {
            this.settingRepository = settingRepository;
        }

        public async Task Send(string toEmail, string subject, string body, bool isBodyHtml, string fromEmail, string fromName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, settingRepository.EmailTo));
            message.To.Add(new MailboxAddress(toEmail, toEmail));

            if (!string.IsNullOrEmpty(fromEmail))
            {
                message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
            }

            message.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;

            message.Body = new TextPart(isBodyHtml ? "html" : "plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(settingRepository.SmtpAddress, settingRepository.SmtpPort, settingRepository.SmtpSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword));
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

    }
}
