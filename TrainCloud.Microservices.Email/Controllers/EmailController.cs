using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrainCloud.Microservices.Core.Controllers;
using TrainCloud.Microservices.Core.Filters.Validation;
using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Messages;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Controllers;

[ApiController]
[Route("/Email/")]
public sealed class EmailController : AbstractController<EmailController>
{
    private IEmailService EmailService { get; init; }

    public EmailController(IWebHostEnvironment webHostEnvironment,
                           IHttpContextAccessor httpContextAccessor,
                           IConfiguration configuration,
                           ILogger<EmailController> logger,
                           IEmailService emailService)
        : base(webHostEnvironment, httpContextAccessor, configuration, logger)
    {
        EmailService = emailService;
    }

    [HttpPost("Send")]
    [Authorize]
    [Consumes("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [ValidateModelFilter(typeof(IValidator<PostSendEmailModel>), typeof(PostSendEmailModel))]
    public async Task<IActionResult> PostAsync([FromBody] PostSendEmailModel postModel)
    {
        string subject = $"ContactForm from {postModel.FromName} {postModel.FromEmail}: {postModel.Subject}";
        await EmailService.SendEmailAsync(new List<string>() { "nico@caratiola.net", "mail@sebastian-hoyer.online" }, null, null, subject, postModel.Body, false, null);
        return Ok();
    }
}