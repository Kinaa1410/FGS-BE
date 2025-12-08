using FluentValidation;

namespace FGS_BE.Repo.DTOs.Levels.Validators
{
    public class CheckLevelUpRequestValidator : AbstractValidator<CheckLevelUpRequest>
    {
        public CheckLevelUpRequestValidator()
        {
            RuleFor(x => x.CurrentPoints)
                .GreaterThanOrEqualTo(0).WithMessage("Current points must be >= 0!");
        }
    }
}