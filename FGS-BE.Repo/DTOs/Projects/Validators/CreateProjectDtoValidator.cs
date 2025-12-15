using FluentValidation;
using FGS_BE.Repo.DTOs.Projects;

namespace FGS_BE.Repo.DTOs.Projects.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Project title cannot be empty.")
                .MaximumLength(200).WithMessage("Project title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Project description cannot be empty.")
                .MaximumLength(5000).WithMessage("Project description cannot exceed 5000 characters.");

            RuleFor(x => x.ProposerId)
                .GreaterThan(0).WithMessage("ProposerId must be greater than 0.");

            RuleFor(x => x.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");

            RuleFor(x => x.MinMembers)
                .GreaterThan(0).WithMessage("MinMembers must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("MinMembers cannot exceed 50.");

            RuleFor(x => x.MaxMembers)
                .GreaterThan(0).WithMessage("MaxMembers must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("MaxMembers cannot exceed 50.");

            RuleFor(x => x)
                .Must(x => x.MinMembers <= x.MaxMembers)
                .WithMessage("MinMembers cannot be greater than MaxMembers.");

            RuleFor(x => x.TotalPoints)
                .GreaterThanOrEqualTo(0).WithMessage("TotalPoints must be non-negative.");

            RuleFor(x => x.MentorId)
                .GreaterThan(0).When(x => x.MentorId.HasValue)
                .WithMessage("MentorId must be greater than 0 if provided.");
        }
    }
}
