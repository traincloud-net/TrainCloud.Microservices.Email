using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrainCloud.Microservices.Core.Controllers;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services;

namespace TrainCloud.Microservices.Email.Controllers;

[ApiController]
[Route("/Email/")]
public class EmailController : AbstractController<EmailController>
{
    private IEmailService EmailService {  get; init; }

    public EmailController(IWebHostEnvironment webHostEnvironment,
                                 IHttpContextAccessor httpContextAccessor,
                                 IConfiguration configuration,
                                 ILogger<EmailController> logger,
                                 IEmailService emailService)
        :base(webHostEnvironment, httpContextAccessor, configuration, logger)
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
            EmailService.SendEmail(postModel);
            return Ok();
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }




}
