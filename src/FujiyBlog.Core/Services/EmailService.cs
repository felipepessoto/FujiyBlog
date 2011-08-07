using System.Net;
using System.Net.Mail;
using FujiyBlog.Core.EntityFramework;
using System;

namespace FujiyBlog.Core.Services
{
    public class EmailService
    {
        public void Send(string fromEmail, string fromName, string subject, string body, bool isBodyHtml)
        {
            SettingRepository settingRepository = new SettingRepository(new FujiyBlogDatabase());

            using (MailMessage mailMessage = new MailMessage(new MailAddress(settingRepository.EmailTo, fromName), new MailAddress(settingRepository.EmailTo)))
            {
                mailMessage.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.ReplyToList.Add(new MailAddress(fromEmail, fromName));

                using (SmtpClient client = new SmtpClient(settingRepository.SmtpAddress, settingRepository.SmtpPort))
                {
                    client.EnableSsl = settingRepository.SmtpSsl;
                    client.Credentials = new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword);
                    client.Send(mailMessage);
                }
            }
        }

        public void SendAsync(string fromEmail, string fromName, string subject, string body, bool isBodyHtml)
        {
            SettingRepository settingRepository = new SettingRepository(new FujiyBlogDatabase());

            MailMessage mailMessage = new MailMessage(new MailAddress(settingRepository.EmailTo, fromName), new MailAddress(settingRepository.EmailTo));
            
            mailMessage.Subject = settingRepository.EmailSubjectPrefix + " - " + subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.ReplyToList.Add(new MailAddress(fromEmail, fromName));

            SmtpClient client = new SmtpClient(settingRepository.SmtpAddress, settingRepository.SmtpPort);
                
            client.EnableSsl = settingRepository.SmtpSsl;
            client.Credentials = new NetworkCredential(settingRepository.SmtpUserName, settingRepository.SmtpPassword);
            client.SendCompleted += client_SendCompleted;
            client.Timeout = 1;
            client.SendAsync(mailMessage, Tuple.Create(client, mailMessage));
        }

        private void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Tuple<SmtpClient, MailMessage> data = (Tuple<SmtpClient, MailMessage>)e.UserState;
            data.Item1.Dispose();
            data.Item2.Dispose();

            if (e.Error != null)
            {
                
            }
        }
    }
}
