using FluentValidation;
using FGS_BE.Repo.DTOs.Semesters;

namespace FGS_BE.Repo.DTOs.Semesters.Validators
{
    public class UpdateSemesterDtoValidator : AbstractValidator<UpdateSemesterDto>
    {
        private static readonly string[] ValidStatuses = { "Upcoming", "Active", "Completed" }; // Define allowed statuses as constants; adjust as needed

        public UpdateSemesterDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Semester name cannot exceed 100 characters.")
                .NotEmpty().When(x => x.Name != null).WithMessage("Name cannot be empty if provided.");

            RuleFor(x => x.KeywordTheme)
                .MaximumLength(200).WithMessage("Keyword theme cannot exceed 200 characters.")
                .NotEmpty().When(x => x.KeywordTheme != null).WithMessage("Keyword theme cannot be empty if provided.");

            // Cross-validation for dates if both provided
            RuleFor(x => x)
                .Must(x =>
                {
                    if (!x.StartDate.HasValue || !x.EndDate.HasValue)
                        return true;
                    return x.StartDate.Value < x.EndDate.Value;
                })
                .WithMessage("Start date must be before end date if both are provided.");

            RuleFor(x => x.StartDate)
                .NotEmpty().When(x => x.StartDate.HasValue)
                .WithMessage("Start date cannot be empty if provided.");

            RuleFor(x => x.EndDate)
                .NotEmpty().When(x => x.EndDate.HasValue)
                .WithMessage("End date cannot be empty if provided.");

            RuleFor(x => x.Status)
                .Must(s => s == null || ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Invalid semester status if provided. Valid values: {string.Join(", ", ValidStatuses)}.");
        }
    }
}