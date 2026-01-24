using FGS_BE.Repo.Resources;
using FluentValidation;

namespace FGS_BE.Repo.DTOs.Users.Validators;
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage(Resource.UsernameRequired);
        RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.PasswordRequired);
    }
}
