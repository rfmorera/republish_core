using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Republish.Data;
using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Models;

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
            //Company company = _context.Set<Company>().Single();

            //SmtpClient client = new SmtpClient(company.ExternalName, company.ExternalPort);
            //client.Timeout = company.ExternalTimeout;
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(company.ExternalUsername, company.ExternalPassword);
            //client.Credentials = credentials;

            //string content = _emailTemplate.Template;
            //content = content.Replace("{EMAIL_CONTENT}", htmlMessage);

            //MailMessage mail = new MailMessage(company.FromEmail, email);
            //mail.Subject = subject;
            //mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html));

            Company company = _context.Set<Company>().Single();

            SmtpClient client = new SmtpClient("http://republishtool.com");
            client.Timeout = company.ExternalTimeout;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("info", "ocvXkyfseq$Npu?in2hd1tbjmrglzx");
            client.Credentials = credentials;

            string content = _emailTemplate.Template;
            content = content.Replace("{EMAIL_CONTENT}", htmlMessage);

            MailMessage mail = new MailMessage("info@republishtool.com", email);
            mail.Subject = subject;
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html));

            await client.SendMailAsync(mail);
        }
    }
}
