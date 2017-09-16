using System.Threading.Tasks;

namespace FujiyBlog.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task SendEmailAsync(string email, string subject, string message, string replyToEmail, string replyToEmailName);
    }
}
