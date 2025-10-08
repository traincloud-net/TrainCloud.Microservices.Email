using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Messages;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Services.MessageBus;

public sealed class EmailMessageBusSubscriber : AbstractMessageBusSubscriber<SendMailMessage>
{
    private IEmailService EmailService { get; init; }

    public EmailMessageBusSubscriber(IConfiguration configuration, 
                                     ILogger<EmailMessageBusSubscriber> logger,
                                     string queueName,
                                     IEmailService emailService) 
        : base(configuration, logger, queueName)
    {
        EmailService = emailService;
    }

    public override async Task OnMessageAsync(SendMailMessage message)
    {
        try
        {
            await EmailService.SendEmailAsync(message.To, message.Cc, message.Cc, message.Subject, message.Body, message.IsHtml, message.Attachments, message.Priority);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }
}
