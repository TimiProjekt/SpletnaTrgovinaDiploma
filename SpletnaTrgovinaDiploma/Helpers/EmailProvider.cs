using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using SpletnaTrgovinaDiploma.Data.Services.Classes;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class EmailProvider
    {
        private static readonly string Sender = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailSender"];
        private static readonly string Password = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailPassword"];

        private const string SmtpAddress = "smtp-relay.brevo.com";
        private const int PortNumber = 587;
        private const bool EnableSsl = true;

        public static void SendEmail(string receiver, string subject, string message)
        {
            var unsubscribeUrl = $"{ConfigurationService.PublishedUrl}/Account/UnsubscribeFromNewsletter?email=";
            var unsubscribeLink = $" <br/> <br/> <a href=\"{unsubscribeUrl}{receiver}\"> Unsubscribe </a> ";

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(Sender);
                mail.To.Add(receiver);
                mail.Subject = subject;
                mail.Body = message + unsubscribeLink;
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

        public static bool IsValidEmail(string email)
        {
            const string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
    }
}
