using FluentValidation;
using TrainCloud.Microservices.Core.Extensions.Authentication;
using TrainCloud.Microservices.Core.Extensions.Swagger;
using TrainCloud.Microservices.Core.Filters.Exception;
using TrainCloud.Microservices.Core.Middleware.LoadBalancing;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;
using TrainCloud.Microservices.Email.Services.MessageBus;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

string environmentName = webApplicationBuilder.Environment.EnvironmentName;

// Login as microservice-email-dev@traincloud.iam.gserviceaccount.com
// Check out and sync the Credentials repository in your local TrainCloud folder
// ./TrainCloud/Credentials/...
// ./TrainCloud/TrainCloud.Microservices.Email/...
if (environmentName == "Local")
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "../../Credentials/serviceaccounts/sa-email-local.json");

    Environment.SetEnvironmentVariable("JWT_ISSUERSIGNINGKEY", Guid.Empty.ToString());
}

webApplicationBuilder.Services.AddAuthorization();
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

webApplication.UseTrainCloudLoadBalancing();
webApplication.UseTrainCloudSwagger();
webApplication.UseAuthorization();
webApplication.MapControllers();

await webApplication.RunAsync();
