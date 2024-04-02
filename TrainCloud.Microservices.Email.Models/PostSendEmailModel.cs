using System.ComponentModel.DataAnnotations;

namespace TrainCloud.Microservices.Email.Models;

public class PostSendEmailModel : IValidatableObject
{
    public List<string> To { get; set; } = new();

    public List<string> CC { get; set; } = new();

    public List<string> BCC { get; set; } = new();

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public Dictionary<string, byte[]> Attachments { get; set; } = new();

    public bool IsHTML { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (this.To.Count < 2)
        {
            yield return new ValidationResult("At least two Email-Addresses are needed in To field");
            
        }
        yield return ValidationResult.Success;
    }
}
