using FujiyBlog.Core.EntityFramework;
using System.Net;
using System.Net.Mail;
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


        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            return SendEmailAsync(toEmail, subject, message, null, null);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, string fromEmail, string fromName)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(settingRepository.EmailTo);
                mailMessage.To.Add(new MailAddress(toEmail));

                if (!string.IsNullOrEmpty(fromEmail))
                {
                    mailMessage.ReplyToList.Add(new MailAddress(fromEmail, fromName));
                }

                mailMessage.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;

                using (var client = new SmtpClient(settingRepository.SmtpAddress, settingRepository.SmtpPort))
                {
                    client.EnableSsl = settingRepository.SmtpSsl;
                    client.Credentials = new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword);
                    await client.SendMailAsync(mailMessage);
                }
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            return Task.FromResult(0);
        }
    }
}
