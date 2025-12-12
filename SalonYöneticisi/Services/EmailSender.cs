using Microsoft.AspNetCore.Identity.UI.Services;


namespace SalonYöneticisi.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Burada gerçek bir e-posta gönderme işlemi gerçekleştirilmelidir.
            return Task.CompletedTask;
        }
    }
}