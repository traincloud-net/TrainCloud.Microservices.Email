using TrainCloud.Microservices.Core.Extensions.Authentication;
using TrainCloud.Microservices.Core.Extensions.Authorization;
using TrainCloud.Microservices.Core.Extensions.Swagger;
using TrainCloud.Microservices.Email.Services.Email;
using TrainCloud.Microservices.Email.Services.MessageBus;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

if (!webApplicationBuilder.Environment.IsProduction())
{
    // Login as microservice-email-dev@traincloud.iam.gserviceaccount.com
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "google.cloud.json");
}

webApplicationBuilder.Services.AddTrainCloudAuthorization();
AuthenticationOptions authenticationOptions = webApplicationBuilder.Configuration.GetSection(AuthenticationOptions.Position).Get<AuthenticationOptions>()!;
webApplicationBuilder.Services.AddTrainCloudAuthentication(authenticationOptions);

SwaggerOptions swaggerOptions = webApplicationBuilder.Configuration.GetSection(SwaggerOptions.Position).Get<SwaggerOptions>()!;
webApplicationBuilder.Services.AddTrainCloudSwagger(swaggerOptions);

webApplicationBuilder.Services.AddControllers();

webApplicationBuilder.Services.AddTransient<IEmailService, EmailService>();

webApplicationBuilder.Services.AddHostedService<NewScanMessageBusSubscriberService>(service =>
    new NewScanMessageBusSubscriberService(service.GetRequiredService<IConfiguration>(),
                                           service.GetRequiredService<ILogger<NewScanMessageBusSubscriberService>>(),
                                           service.GetRequiredService<IServiceScopeFactory>(),
                                           webApplicationBuilder.Configuration.GetValue<string>("MessageBus:Subscriptions:Email")!,
                                           service.GetRequiredService<IEmailService>()));

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
