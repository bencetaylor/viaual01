using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace LunchTrain.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private const string MailUser = "";
        private const string MailPass = "";
        // todo actual user pass on outlook.com

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if(string.IsNullOrWhiteSpace(MailUser) || string.IsNullOrWhiteSpace(MailPass)) return;
            var msg = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart("plain")
                {
                    Text = message
                }
            };
            msg.From.Add(new MailboxAddress(Encoding.UTF8, "LunchTrain", "noreply@lunchtrain.com"));
            msg.To.Add(new MailboxAddress(Encoding.UTF8, "LunchTrain user", email));
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.live.com", 587);
                client.Authenticate(MailUser, MailPass);
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
            }
        }
    }
}
