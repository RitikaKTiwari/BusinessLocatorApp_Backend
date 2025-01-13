using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
