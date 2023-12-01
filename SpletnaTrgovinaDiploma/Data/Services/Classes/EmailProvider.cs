using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public static class EmailProvider
    {
        private static readonly string Sender = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailSender"];
        private static readonly string Password = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailPassword"];

        private const string SmtpAddress = "smtp-relay.sendinblue.com";
        private const int PortNumber = 587;
        private const bool EnableSsl = true;

        public static void SendEmail(string receiver, string subject, string message)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(Sender);
                mail.To.Add(receiver);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                using (var smtp = new SmtpClient(SmtpAddress, PortNumber))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(Sender, Password);
                    smtp.EnableSsl = EnableSsl;
                    smtp.Send(mail);
                }
            }
        }
    }
}
