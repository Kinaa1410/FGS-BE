using FluentValidation;
using FGS_BE.Repo.DTOs.RedeemRequests;

namespace FGS_BE.Repo.DTOs.RedeemRequests.Validators;

public class CreateRedeemRequestDtoValidator
    : AbstractValidator<CreateRedeemRequestDto>
{
    public CreateRedeemRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.RewardItemId)
            .GreaterThan(0)
            .WithMessage("RewardItemId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1.");
    }
}
