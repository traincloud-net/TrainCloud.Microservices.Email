using FluentValidation;
using Prometheus;
using TrainCloud.Microservices.Core.Extensions.Authentication;
using TrainCloud.Microservices.Core.Extensions.Kestrel;
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
    webApplicationBuilder.WebHost.ConfigureTrainCloudKestrel(7098, true, false, false);
}
else
{
    string microservicePort = Environment.GetEnvironmentVariable("TRAINCLOUD_MICROSERVICE_PORT")!;
    string microserviceTlsEnabled = Environment.GetEnvironmentVariable("TRAINCLOUD_MICROSERVICE_TLS_ENABLED")!;
    int port = int.Parse(microservicePort);
    bool tlsEnabled = bool.Parse(microserviceTlsEnabled);
    webApplicationBuilder.WebHost.ConfigureTrainCloudKestrel(port, tlsEnabled, true, true);
}

webApplicationBuilder.Services.AddAuthorization();
AuthenticationOptions authenticationOptions = webApplicationBuilder.Configuration.GetSection(AuthenticationOptions.Position).Get<AuthenticationOptions>()!;
webApplicationBuilder.Services.AddTrainCloudAuthentication(authenticationOptions);

webApplicationBuilder.Services.AddHealthChecks();
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

if (webApplication.Environment.IsProduction())
{
    // Prometheus 
    webApplication.UseHttpMetrics();
    webApplication.MapMetrics()
                  .RequireHost("*:9090");

    // k8s health checks
    webApplication.MapHealthChecks("/health")
                  .RequireHost("*:8080");
}

webApplication.Use((context, next) =>
{
    string k8sNamespace = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_NAMESPACE") ?? "Development";
    context.Response.Headers.Append("traincloud-service-namespace", k8sNamespace);

    string k8sNode = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_NODE") ?? "Development";
    string k8sNodeShort = k8sNode.Substring(k8sNode.Length - 10);
    context.Response.Headers.Append("traincloud-service-node", k8sNodeShort);

    string serviceVersion = Environment.GetEnvironmentVariable("TRAINCLOUD_SERVICE_VERSION") ?? "Development";
    context.Response.Headers.Append("traincloud-service-version", serviceVersion);

    return next.Invoke();
});

webApplication.UseTrainCloudLocalization();
webApplication.UseTrainCloudSwagger();
webApplication.UseAuthorization();
webApplication.MapControllers();

await webApplication.RunAsync();
