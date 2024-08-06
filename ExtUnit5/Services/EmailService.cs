using Mailhog;
using Microsoft.AspNetCore.Components;
using System.Net.Mail;

namespace ExtUnit5.Services
{
    public class EmailService
    {

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient("localhost", 1025))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("ecommerce-shop@example.com");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    await client.SendMailAsync(mailMessage);
                }
            }
        }

        public async Task GetEmails()
        {
            var mailhogClient = new MailhogClient(new Uri("http://localhost:8025/api/v2/messages"));
            var messages = await mailhogClient.GetMessagesAsync();
            var subjects = messages.Items.Select(m => m.Subject);
        }
    }
}
