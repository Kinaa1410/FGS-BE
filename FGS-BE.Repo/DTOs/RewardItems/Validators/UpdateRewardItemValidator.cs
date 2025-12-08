using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.RewardItems.Validators
{

    public class UpdateRewardItemValidator : AbstractValidator<UpdateRewardItemDto>
    {
        public UpdateRewardItemValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(255)
                .WithMessage("Name cannot exceed 255 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.PriceInPoints)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .When(x => x.PriceInPoints.HasValue);

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be greater than or equal to 0.")
                .When(x => x.Quantity.HasValue);

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid reward item type.")
                .When(x => x.Type.HasValue);
        }
    }
}
