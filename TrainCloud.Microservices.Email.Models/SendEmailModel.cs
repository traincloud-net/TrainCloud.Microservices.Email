using System.ComponentModel.DataAnnotations;

namespace TrainCloud.Microservices.Email.Models;

public class SendEmailModel
{
    public string[]? To { get; set; }

    public string[]? CC { get; set; }

    public string[]? BCC { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public string[]? AttachmentFilePaths { get; set; }

    public bool IsHTML { get; set; }
}
