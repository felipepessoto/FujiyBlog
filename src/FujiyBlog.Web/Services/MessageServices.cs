using FujiyBlog.Core.EntityFramework;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly SettingRepository settingRepository;

        public AuthMessageSender(SettingRepository settingRepository)
        {
            this.settingRepository = settingRepository;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(settingRepository.EmailTo));
            mimeMessage.To.Add(new MailboxAddress(toEmail));

            //if (!string.IsNullOrEmpty(fromEmail))
            //{
            //    message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
            //}

            mimeMessage.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;

            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(settingRepository.SmtpAddress, settingRepository.SmtpPort, settingRepository.SmtpSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword));
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
