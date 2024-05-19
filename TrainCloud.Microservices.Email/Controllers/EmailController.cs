using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrainCloud.Microservices.Core.Controllers;
using TrainCloud.Microservices.Core.Filters.Validation;
using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Controllers;

[ApiController]
[Route("/Email/")]
public sealed class EmailController : AbstractController<EmailController>
{
    private IEmailService EmailService { get; init; }
    private IMessageBusPublisherService MessageBusPublisherService { get; init; }


    public EmailController(IWebHostEnvironment webHostEnvironment,
                           IHttpContextAccessor httpContextAccessor,
                           IConfiguration configuration,
                           ILogger<EmailController> logger,
                           IEmailService emailService,
                           IMessageBusPublisherService messageBusPublisherService)
        : base(webHostEnvironment, httpContextAccessor, configuration, logger)
    {
        EmailService = emailService;
        MessageBusPublisherService = messageBusPublisherService;
    }

    [HttpPost("Send")]
    [Authorize]
    [Consumes("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [ValidateModelFilter(typeof(IValidator<PostSendEmailModel>), typeof(PostSendEmailModel))]
    public async Task<IActionResult> PostAsync([FromBody] PostSendEmailModel postModel)
    {
        await EmailService.SendEmailAsync(new List<string>() { "nico@caratiola.net", "mail@sebastian-hoyer.online" }, null, null, postModel.Subject, postModel.Body, false, null);
        return Ok();
    }

    [HttpPost("Test")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostTestAsync()
    {
        string topicId = Configuration.GetValue<string>("MessageBus:Topics:Email")!;
        var busMessage = new Email.Messages.SendMailMessage()
        {
            To = new List<string> { "mail@sebastian-hoyer.online" },
            Subject = "TrainCloud email testmail",
            Body = $"Hallo",
            IsHtml = true,
        };
        await MessageBusPublisherService.SendMessageAsync(topicId, busMessage);
        return Ok();
    }
}