using FGS_BE.Repo.Resources;
using FluentValidation;

namespace FGS_BE.Repo.DTOs.Users.Validators;
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .EmailAddress().WithMessage(Resource.InvalidEmail)
            .NotEmpty().WithMessage(Resource.UsernameRequired);
        RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.PasswordRequired);
    }
}
