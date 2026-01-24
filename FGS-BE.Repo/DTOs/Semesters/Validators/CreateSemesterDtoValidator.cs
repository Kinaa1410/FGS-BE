using FluentValidation;
using FGS_BE.Repo.DTOs.Semesters;

namespace FGS_BE.Repo.DTOs.Semesters.Validators
{
    public class CreateSemesterDtoValidator : AbstractValidator<CreateSemesterDto>
    {
        private static readonly string[] ValidStatuses = { "Upcoming", "Active", "Completed" }; // Define allowed statuses as constants; adjust as needed

        public CreateSemesterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Semester name cannot be empty.")
                .MaximumLength(100).WithMessage("Semester name cannot exceed 100 characters.");

            RuleFor(x => x.KeywordTheme)
                .MaximumLength(200).WithMessage("Keyword theme cannot exceed 200 characters.")
                .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.KeywordTheme))
                .WithMessage("Keyword theme cannot be empty if provided.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status cannot be empty.")
                .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Invalid semester status. Valid values: {string.Join(", ", ValidStatuses)}.");
        }
    }
}