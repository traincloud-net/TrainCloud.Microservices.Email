using System.ComponentModel.DataAnnotations;

namespace TrainCloud.Microservices.Email.Models;

public class PostSendEmailModel 
{
    public string FromName { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}
