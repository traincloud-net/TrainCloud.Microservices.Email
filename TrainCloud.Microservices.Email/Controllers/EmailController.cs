using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrainCloud.Microservices.Core.Controllers;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;

namespace TrainCloud.Microservices.Email.Controllers;

[ApiController]
[Route("/Email/")]
public class EmailController : AbstractController<EmailController>
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
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] PostSendEmailModel postModel)
    {
        try
        {
            await EmailService.SendEmailAsync(postModel.To, postModel.Cc, postModel.Bcc, postModel.Subject, postModel.Body, postModel.IsHtml, postModel.Attachments);
            return Ok();
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
}