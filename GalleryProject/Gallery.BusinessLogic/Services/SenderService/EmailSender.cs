using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.BusinessLogic.Services.SenderService
{
    public sealed class EmailSender: IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Console logging for demonstration 
            Console.WriteLine($"\n--- EMAIL TO: {email} ---\nSUBJECT: {subject}\n{htmlMessage}\n------------------\n");

            /* // Example of sending (Gmail SMTP)
            var mail = "твій_email@gmail.com";
            var pw = "твій_пароль_додатка"; // Створюється в налаштуваннях Google (App Passwords)

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            using var message = new MailMessage(from: mail, to: email, subject, htmlMessage) { IsBodyHtml = true };
            await client.SendMailAsync(message);
            */

            await Task.CompletedTask;
        }
    }
}
