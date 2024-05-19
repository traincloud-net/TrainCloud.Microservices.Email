using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace TrainCloud.Microservices.Email.Models;

public class PostSendEmailModelValidator : AbstractValidator<PostSendEmailModel>
{
    public PostSendEmailModelValidator(IConfiguration configuration,
                                       IStringLocalizer<PostSendEmailModelValidator> localizer)
    {
        //RuleFor(signIn => signIn.UserName).NotNull()
        //                                  .WithMessage("Der Wolf; das Lamm...");
    }
}
