using Microsoft.Extensions.Configuration;

namespace SpletnaTrgovinaDiploma.Data.Services.Classes
{
    public static class ConfigurationService
    {
        public static readonly string Sender = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailSender"];

        public static readonly string Password = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["EmailPassword"];

        public static readonly string PublishedUrl = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetSection("AppSettings")["PublishedUrl"];
    }
}
