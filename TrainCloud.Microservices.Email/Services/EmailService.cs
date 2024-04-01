using System.Net.Mail;
using System.Net;
using TrainCloud.Microservices.Core.Services;
using TrainCloud.Microservices.Email.Models;
using Microsoft.IdentityModel.Tokens;

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
        using MailMessage mail = new MailMessage();

        mail.From = new MailAddress("mail@traincloud.net", "TrainCloud");


        foreach (string addr in (postModel.To))
        {
            mail.To.Add(new MailAddress(addr));
        }



        foreach (string addr in (postModel.CC))
        {
            mail.CC.Add(new MailAddress(addr));
        }




        foreach (string addr in (postModel.BCC))
        {
            mail.Bcc.Add(new MailAddress(addr));
        }



        mail.IsBodyHtml = postModel.IsHTML;
        mail.Subject = postModel.Title;
        mail.Body = postModel.Body;

        if (postModel.Attachments.Count > 0)
        {
            foreach (KeyValuePair<string, byte[]> attachment in postModel.Attachments)
            {


                var stream = new MemoryStream(attachment.Value);
                var a = new Attachment(stream, attachment.Key, null);
                mail.Attachments.Add(a);

            }
        }

        using (SmtpClient client = new SmtpClient("smtp.ionos.de", 587))
        {
            client.Credentials = new NetworkCredential("mail@traincloud.net", "9N!@&^T5F5V5agw");
            client.EnableSsl = true;

            client.Send(mail);
        };


    }
}

//{
//  "to": [
//    ""
//  ],
//  "title": "Mailer",
//  "body": "Mail \n with tab",
//  "attachmentFilePaths": [
//    "C:/Users/test/Desktop/test.docx"
//  ],
//  "isHTML": true
//}
