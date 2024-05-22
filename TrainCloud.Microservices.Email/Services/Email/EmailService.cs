using System.Net.Mail;
using System.Net;
using TrainCloud.Microservices.Core.Services;

namespace TrainCloud.Microservices.Email.Services.Email;

public sealed class EmailService : AbstractService<EmailService>, IEmailService
{
    public EmailService(IConfiguration configuration,
                        ILogger<EmailService> logger)
        : base(configuration, logger)
    {

    }

    public async Task SendEmailAsync(List<string>? to,
                                     List<string>? cc,
                                     List<string>? bcc,
                                     string subject,
                                     string body,
                                     bool isBodyHtml,
                                     Dictionary<string, byte[]>? attachments)
    {

        using MailMessage mail = new MailMessage();

        mail.From = new MailAddress("mail@traincloud.net", "TrainCloud");

        if (to is not null)
        {
            foreach (string toAddress in to)
            {
                mail.To.Add(new MailAddress(toAddress));
            }
        }

        if (cc is not null)
        {
            foreach (string ccAdress in cc)
            {
                mail.CC.Add(new MailAddress(ccAdress));
            }
        }

        if (bcc is not null)
        {
            foreach (string bccAddress in bcc)
            {
                mail.Bcc.Add(new MailAddress(bccAddress));
            }
        }

        mail.IsBodyHtml = isBodyHtml;
        mail.Subject = subject;
        mail.Body = body;

        if (attachments is not null && attachments.Count > 0)
        {
            foreach (KeyValuePair<string, byte[]> attachment in attachments)
            {
                var stream = new MemoryStream(attachment.Value);
                var a = new Attachment(stream, attachment.Key, null);
                mail.Attachments.Add(a);
            }
        }

        using SmtpClient client = new SmtpClient("smtp.ionos.de", 587);
        client.Credentials = new NetworkCredential("mail@traincloud.net", "9N!@&^T5F5V5agw");
        client.EnableSsl = true;

        await client.SendMailAsync(mail);
    }
}
