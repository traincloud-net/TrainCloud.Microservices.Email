namespace TrainCloud.Microservices.Email.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(List<string>? to,
                        List<string>? cc,
                        List<string>? bcc,
                        string subject,
                        string body,
                        bool isBodyHtml,
                        Dictionary<string, byte[]>? attachments);
}

