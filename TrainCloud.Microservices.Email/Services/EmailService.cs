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
        var client = new SmtpClient("smtp.ionos.de", 587)
        {
            Credentials = new NetworkCredential("mail@traincloud.net", "9N!@&^T5F5V5agw"),
            EnableSsl = true
        };

        var mail = new MailMessage(new MailAddress("mail@traincloud.net", "TrainCloud"), new MailAddress("myMailAdress")) //mail@sebastian-hoyer.online"))
        {
            Subject = "TrainCloud Wagenliste",
            Body = $"Have a look at your brand new Wagenliste! 🚃",
        };
    }
}
