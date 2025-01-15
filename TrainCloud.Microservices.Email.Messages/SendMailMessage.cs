using System.ComponentModel.DataAnnotations;

namespace TrainCloud.Microservices.Email.Messages;

public class SendMailMessage
{
    public List<string>? To { get; set; } = new();

    public List<string>? Cc { get; set; } = new();

    public List<string>? Bcc { get; set; } = new();

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; }

    public Dictionary<string, byte[]>? Attachments { get; set; }

    /// <summary>
    /// 0 = normal, 1 = Low, 2 = High
    /// </summary>
    public byte Priority { get; set; } = 0;
}

