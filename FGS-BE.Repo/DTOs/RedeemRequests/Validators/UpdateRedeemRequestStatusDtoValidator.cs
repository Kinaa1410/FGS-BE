using FluentValidation;
using FGS_BE.Repo.DTOs.RedeemRequests;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.RedeemRequests.Validators
{
    public class UpdateStatusRedeemRequestDtoValidator
        : AbstractValidator<UpdateStatusRedeemRequestDto>
    {
        public UpdateStatusRedeemRequestDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid redeem request status.");
        }
    }
}
