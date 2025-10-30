using FluentValidation;
using TrainCloud.Microservices.Core.Extensions.Authentication;
using TrainCloud.Microservices.Core.Extensions.Swagger;
using TrainCloud.Microservices.Core.Filters.Exception;
using TrainCloud.Microservices.Core.Middleware.Localization;
using TrainCloud.Microservices.Core.Services.MessageBus;
using TrainCloud.Microservices.Email.Models;
using TrainCloud.Microservices.Email.Services.Email;
using TrainCloud.Microservices.Email.Services.MessageBus;

var webApplicationBuilder = WebApplication.CreateBuilder(args);
if (!webApplicationBuilder.Environment.IsProduction())
{
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
    controllerOptions.Filters.Add<GlobalExceptionFilterAttribute>();
});

webApplicationBuilder.Services.AddScoped<MessageBusPublisher>();
webApplicationBuilder.Services.AddHostedService<EmailMessageBusSubscriber>(serviceProvider =>
    new EmailMessageBusSubscriber(serviceProvider.GetRequiredService<IConfiguration>(),
                                  serviceProvider.GetRequiredService<ILogger<EmailMessageBusSubscriber>>(),
                                  "email",
                                  serviceProvider.GetRequiredService<IEmailService>()));

webApplicationBuilder.Services.AddScoped<IValidator<PostSendEmailModel>, PostSendEmailModelValidator>();

WebApplication webApplication = webApplicationBuilder.Build();

webApplication.Use(async (context, next) =>
{
    string trainCloudEnvironment = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_ENVIRONMENT") ?? "Development";
    context.Response.Headers.Append("TrainCloud-Service-Environment", trainCloudEnvironment);
    await next.Invoke();
});

webApplication.Use(async (context, next) =>
{
    string k8sNamespace = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_NAMESPACE") ?? "Development";
    context.Response.Headers.Append("traincloud-service-namespace", k8sNamespace);

    string k8sNode = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_NODE") ?? "Development";
    string k8sNodeShort = k8sNode.Substring(k8sNode.Length - 10);
    context.Response.Headers.Append("traincloud-service-node", k8sNodeShort);

    string serviceVersion = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_VERSION") ?? "Development";
    context.Response.Headers.Append("traincloud-service-version", serviceVersion);

    await next.Invoke();
});

webApplication.UseTrainCloudLocalization();
webApplication.UseTrainCloudSwagger();
webApplication.UseAuthorization();
webApplication.MapControllers();

await webApplication.RunAsync();
