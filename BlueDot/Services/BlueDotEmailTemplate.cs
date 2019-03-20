using System.IO;
using Republish.Extensions;
using Microsoft.Extensions.Configuration;
using Services;

namespace Republish.Services
{
    public class RepublishEmailTemplate : IEmailTemplate
    {
        public string Template { get; }

        public RepublishEmailTemplate(IConfiguration configuration)
        {
            Template = File.ReadAllText(configuration.GetEmailTemplateFilePath());
        }
    }
}