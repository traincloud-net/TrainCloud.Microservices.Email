using System.Net;
using System.Net.Mail;
using TrainCloud.Microservices.Core.Services;

namespace TrainCloud.Microservices.Email.Services.Email;

public sealed class EmailService : AbstractService<EmailService>, IEmailService
{
    private MailAddress TrainCloudMailAddress { get; } = new("mail@traincloud.net", "TrainCloud");

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
                                     Dictionary<string, byte[]>? attachments,
                                     byte priority = 0)
    {
        if(string.IsNullOrEmpty(subject))
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (string.IsNullOrEmpty(body))
        {
            throw new ArgumentNullException(nameof(body));
        }

        

        using MailMessage mail = new MailMessage();
        mail.From = TrainCloudMailAddress;

        if (to is not null)
        {
            foreach (string toAddress in to)
            {
                if (toAddress is not null)
                {
                    mail.To.Add(new MailAddress(toAddress));
                }
            }
        }

        if (cc is not null)
        {
            foreach (string ccAdress in cc)
            {
                if (ccAdress is not null)
                {
                    mail.CC.Add(new MailAddress(ccAdress));
                }
            }
        }

        if (bcc is not null)
        {
            foreach (string bccAddress in bcc)
            {
                if (bccAddress is not null)
                {
                    mail.Bcc.Add(new MailAddress(bccAddress));
                }
            }
        }

        mail.Priority = (MailPriority) priority;
        mail.IsBodyHtml = isBodyHtml;
        mail.Subject = subject;
        mail.Body = body;

        if (attachments is not null && attachments.Count > 0)
        {
            foreach (KeyValuePair<string, byte[]> attachment in attachments)
            {
                if (!string.IsNullOrEmpty(attachment.Key) && attachment.Value.Length > 0)
                {
                    var stream = new MemoryStream(attachment.Value);
                    var a = new Attachment(stream, attachment.Key, null);
                    mail.Attachments.Add(a);
                }
                else
                {
                    throw new ArgumentException("Attachment is not valid.");
                }
            }
        }

        string? emailServer = Environment.GetEnvironmentVariable("EMAIL_SERVER");
        string? emailUserName = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
        string? emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        int emailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!);

        using SmtpClient client = new SmtpClient(emailServer, emailPort);
        client.Credentials = new NetworkCredential(emailUserName, emailPassword);
        client.EnableSsl = true;

        await client.SendMailAsync(mail);
    }
}
