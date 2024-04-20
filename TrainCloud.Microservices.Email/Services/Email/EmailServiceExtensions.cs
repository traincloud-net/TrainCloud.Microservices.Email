using System.Runtime.CompilerServices;

namespace TrainCloud.Microservices.Email.Services.Email;

public static class EmailServiceExtensions
{
    public static IServiceCollection AddEmailService( this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        return services;
    }
}
