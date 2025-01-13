using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IEmailService _emailService;

        public ContactUsService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendContactUsMessageAsync(string name, string email, string message)
        {
            // Construct the email body
            string emailBody = $"<h2>Contact Us Message</h2><br>" +
                               $"<strong>Name:</strong> {name}<br>" +
                               $"<strong>Email:</strong> {email}<br>" +
                               $"<strong>Message:</strong><br>{message}";

            // Send the email to your admin or business email
            await _emailService.SendEmailAsync("rktiwari.surat@gmail.com", "New Contact Us Message", emailBody);
        }
    }
}