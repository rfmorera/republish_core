using Microsoft.Extensions.Configuration;

namespace Republish.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetEmailTemplateFilePath(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("EmailTemplateFilePath");
        }
    }
}