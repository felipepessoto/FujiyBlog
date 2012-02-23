using FujiyBlog.Core.Services;

namespace FujiyBlog.Core.Tasks
{
    public class SendEmailTask: BackgroundTask
    {
        private readonly string toEmail;
        private readonly string subject;
        private readonly string body;
        private readonly string fromEmail;
        private readonly string fromName;

        public SendEmailTask(string toEmail, string subject, string body):this(toEmail, subject, body, null, null)
        {

        }

        public SendEmailTask(string toEmail, string subject, string body, string fromEmail, string fromName)
        {
            this.toEmail = toEmail;
            this.subject = subject;
            this.body = body;
            this.fromEmail = fromEmail;
            this.fromName = fromName;
        }

        protected override void Execute()
        {
            EmailService.Send(toEmail, subject, body, true, fromEmail, fromName);
        }
    }
}
