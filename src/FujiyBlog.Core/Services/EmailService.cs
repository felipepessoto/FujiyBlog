using System.Net;
using System.Net.Mail;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Core.Services
{
    public static class EmailService
    {
        //public static void Send(string toEmail, string subject, string body, bool isBodyHtml)
        //{
        //    Send(toEmail, subject, body, isBodyHtml, null, null);
        //}

        public static void Send(string toEmail, string subject, string body, bool isBodyHtml, string fromEmail, string fromName)
        {
            SettingRepository settingRepository = new SettingRepository(new FujiyBlogDatabase());

            using (MailMessage mailMessage = new MailMessage(new MailAddress(settingRepository.EmailTo, fromName), new MailAddress(toEmail)))
            {
                mailMessage.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;

                if (!string.IsNullOrEmpty(fromEmail))
                {
                    mailMessage.ReplyToList.Add(new MailAddress(fromEmail, fromName));
                }

                using (SmtpClient client = new SmtpClient(settingRepository.SmtpAddress, settingRepository.SmtpPort))
                {
                    client.EnableSsl = settingRepository.SmtpSsl;
                    client.Credentials = new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword);
                    client.Send(mailMessage);
                }
            }
        }
    }
}
