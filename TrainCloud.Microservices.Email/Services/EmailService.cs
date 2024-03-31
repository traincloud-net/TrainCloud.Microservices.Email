using System.Net.Mail;
using System.Net;
using TrainCloud.Microservices.Core.Services;
using TrainCloud.Microservices.Email.Models;

namespace TrainCloud.Microservices.Email.Services;

public class EmailService : AbstractService<EmailService>, IEmailService
{
    public EmailService(IConfiguration configuration,
                        ILogger<EmailService> logger)
        : base(configuration, logger)
    {

    }

    public void SendEmail(SendEmailModel postModel)
    {
        using (MailMessage mail = new MailMessage())
        {
            mail.From = new MailAddress("mail@traincloud.net", "TrainCloud");
            mail.To.Add(new MailAddress(postModel.To));
            if (postModel.CC != null) mail.CC.Add(new MailAddress(postModel.CC));
            if (postModel.BCC != null) mail.Bcc.Add(new MailAddress(postModel.BCC));
            mail.IsBodyHtml = postModel.IsHTML;
            mail.Subject = postModel.Title;
            mail.Body = postModel.Body;

            foreach (var AttachmentFilePath in postModel.AttachmentFilePaths)
            {
                if (File.Exists(AttachmentFilePath))
                {
                    Attachment a = new Attachment(AttachmentFilePath);
                    mail.Attachments.Add(a);
                }
            }

            using (SmtpClient client = new SmtpClient("smtp.ionos.de", 587))
            {
                client.Credentials = new NetworkCredential("mail@traincloud.net", "9N!@&^T5F5V5agw");
                client.EnableSsl = true;
                client.Send(mail);
            };
        };

        
    }
}
