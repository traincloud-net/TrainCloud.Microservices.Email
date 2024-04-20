using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Messages;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Services.MessageBus;

public sealed class NewScanMessageBusSubscriberService : AbstractMessageBusSubscriberService<SendMailMessage>
{
    private IEmailService EmailService { get; init; }

    public NewScanMessageBusSubscriberService(IConfiguration configuration,
                                              ILogger<NewScanMessageBusSubscriberService> logger,
                                              IServiceScopeFactory serviceScopeFactory,
                                              string subscriptionId,
                                              IEmailService emailService)
        : base(configuration, logger, serviceScopeFactory, subscriptionId)
    {
        EmailService = emailService;
    }

    public override async Task OnMessageAsync(SendMailMessage message) 
    {
        //using var scope = ServiceScopeFactory.CreateScope();

        await EmailService.SendEmailAsync(message.To, message.Cc, message.Cc, message.Subject, message.Body, message.IsHtml, message.Attachments);
    }
}
