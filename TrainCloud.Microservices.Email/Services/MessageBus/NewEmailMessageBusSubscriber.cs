using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Messages;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Services.MessageBus;

public sealed class NewEmailMessageBusSubscriber : AbstractMessageBusSubscriber<SendMailMessage>
{
    private IEmailService EmailService { get; init; }

    public NewEmailMessageBusSubscriber(IConfiguration configuration, 
                                        ILogger<NewEmailMessageBusSubscriber> logger,
                                        IEmailService emailService) 
        : base(configuration, logger)
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
