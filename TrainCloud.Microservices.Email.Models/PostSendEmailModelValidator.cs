using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace TrainCloud.Microservices.Email.Models;

public class PostSendEmailModelValidator : AbstractValidator<PostSendEmailModel>
{
    public PostSendEmailModelValidator(IConfiguration configuration,
                                       IStringLocalizer<PostSendEmailModelValidator> localizer)
    {
        RuleFor(signIn => signIn.FromEmail).EmailAddress().WithMessage(localizer["EMAIL"]);
        RuleFor(signIn => signIn.FromName).MinimumLength(3);
        RuleFor(signIn => signIn.FromName).MaximumLength(200);
        RuleFor(signIn => signIn.Subject).MinimumLength(3);
        RuleFor(signIn => signIn.Subject).MaximumLength(200);
        RuleFor(signIn => signIn.Body).MinimumLength(3);
    }
}
