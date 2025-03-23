using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace TechSupportXPress.Brokers
{
    public class SESEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SESEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient
            {
                Host = _configuration["EmailSettings:SmtpServer"],
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            var res = smtpClient.SendMailAsync(mailMessage);
            return res;
        }
    }
}
