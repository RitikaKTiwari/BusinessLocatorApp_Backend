using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BusinessLocatorApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Read settings from appsettings.json
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using (var smtpClient = new SmtpClient(smtpServer, port))
            {
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true; // Ensures a secure connection

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if sending HTML emails
                };

                mailMessage.To.Add(to);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    // Log the error or handle it as needed
                    throw new Exception("Email sending failed. Please check your SMTP settings.", ex);
                }
            }
        }





    }
}