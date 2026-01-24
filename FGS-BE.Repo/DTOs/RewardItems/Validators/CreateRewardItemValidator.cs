using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.RewardItems.Validators
{
    public class CreateRewardItemValidator : AbstractValidator<CreateRewardItemDto>
    {
        public CreateRewardItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.PriceInPoints)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be >= 0.");

            //RuleFor(x => x.Type)
            //    .IsInEnum().WithMessage("Invalid reward item type.");
        }
    }
}
