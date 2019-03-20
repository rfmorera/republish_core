using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Republish.Data;
using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Services.Impls
{
    public class EmailSender : IEmailSender
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailTemplate _emailTemplate;

        public EmailSender(ApplicationDbContext context, IEmailTemplate emailTemplate)
        {
            _context = context;
            _emailTemplate = emailTemplate;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Company lawFirm = _context.Set<Company>().Single();

            SmtpClient client = new SmtpClient(lawFirm.ExternalName, lawFirm.ExternalPort);
            client.Timeout = lawFirm.ExternalTimeout;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(lawFirm.ExternalUsername, lawFirm.ExternalPassword);
            client.Credentials = credentials;

            string content = _emailTemplate.Template;
            content = content.Replace("{EMAIL_CONTENT}", htmlMessage);

            MailMessage mail = new MailMessage(lawFirm.FromEmail, email);
            mail.Subject = subject;
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html));

            await client.SendMailAsync(mail);
        }
    }
}
