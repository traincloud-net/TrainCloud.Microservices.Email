using FluentValidation;
using TrainCloud.Microservices.Core.Extensions.Authentication;
using TrainCloud.Microservices.Core.Extensions.Authorization;
using TrainCloud.Microservices.Core.Extensions.Swagger;
using TrainCloud.Microservices.Core.Filters.Exception;
using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;
using TrainCloud.Microservices.Email.Services.MessageBus;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

if (!webApplicationBuilder.Environment.IsProduction())
{
    // Login as microservice-email-dev@traincloud.iam.gserviceaccount.com
    // Check out and sync the Credentials repository in your local TrainCloud folder
    // ./TrainCloud/Credentials/...
    // ./TrainCloud/TrainCloud.Microservices.Email/...
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "../../Credentials/serviceaccounts/microservice-email-dev.json");

    Environment.SetEnvironmentVariable("JWT_ISSUERSIGNINGKEY", Guid.Empty.ToString());
}

webApplicationBuilder.Services.AddTrainCloudAuthorization();
AuthenticationOptions authenticationOptions = webApplicationBuilder.Configuration.GetSection(AuthenticationOptions.Position).Get<AuthenticationOptions>()!;
webApplicationBuilder.Services.AddTrainCloudAuthentication(authenticationOptions);

webApplicationBuilder.Services.AddHttpContextAccessor();

webApplicationBuilder.Services.AddLocalization();

SwaggerOptions swaggerOptions = webApplicationBuilder.Configuration.GetSection(SwaggerOptions.Position).Get<SwaggerOptions>()!;
webApplicationBuilder.Services.AddTrainCloudSwagger(swaggerOptions);

webApplicationBuilder.Services.AddEmailService();
webApplicationBuilder.Services.AddControllers(controllerOptions =>
{
    controllerOptions.Filters.Add<GlobalExceptionFilter>();
});

webApplicationBuilder.Services.AddHostedService<NewEmailMessageBusSubscriberService>(service =>
    new NewEmailMessageBusSubscriberService(service.GetRequiredService<IConfiguration>(),
                                            service.GetRequiredService<ILogger<NewEmailMessageBusSubscriberService>>(),
                                            service.GetRequiredService<IServiceScopeFactory>(),
                                            webApplicationBuilder.Configuration.GetValue<string>("MessageBus:Subscriptions:Email")!,
                                            service.GetRequiredService<IEmailService>()));

webApplicationBuilder.Services.AddScoped<IValidator<PostSendEmailModel>, PostSendEmailModelValidator>();

WebApplication webApplication = webApplicationBuilder.Build();

webApplication.UseTrainCloudSwagger();
webApplication.UseAuthorization();
webApplication.MapControllers();
webApplication.Run();

/// <summary>
/// The class definition is required to make this service testable
/// TrainCloud.Tests.Microservices.Email requires a visible Program class for the WebApplicationFactory
/// </summary>
public partial class Program { }
