using TrainCloud.Microservices.Email.Models;

namespace TrainCloud.Microservices.Email.Services;

public interface IEmailService
{
    void SendEmail(PostSendEmailModel postModel);
}

