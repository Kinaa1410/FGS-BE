using FluentValidation;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.Projects.Validators
{
    public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Project title cannot exceed 200 characters.")
                .NotEmpty().When(x => x.Title != null).WithMessage("Title cannot be empty if provided.");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Project description cannot exceed 5000 characters.")
                .NotEmpty().When(x => x.Description != null).WithMessage("Description cannot be empty if provided.");

            RuleFor(x => x.TotalPoints)
                .GreaterThanOrEqualTo(0).When(x => x.TotalPoints.HasValue)
                .WithMessage("TotalPoints must be non-negative.");

            RuleFor(x => x.MinMembers)
                .GreaterThan(0).When(x => x.MinMembers.HasValue)
                .WithMessage("MinMembers must be greater than 0.")
                .LessThanOrEqualTo(50).When(x => x.MinMembers.HasValue)
                .WithMessage("MinMembers cannot exceed 50.");

            RuleFor(x => x.MaxMembers)
                .GreaterThan(0).When(x => x.MaxMembers.HasValue)
                .WithMessage("MaxMembers must be greater than 0.")
                .LessThanOrEqualTo(50).When(x => x.MaxMembers.HasValue)
                .WithMessage("MaxMembers cannot exceed 50.");

            // Relationship between min and max
            RuleFor(x => x)
                .Must(x =>
                {
                    if (!x.MinMembers.HasValue || !x.MaxMembers.HasValue)
                        return true;
                    return x.MinMembers.Value <= x.MaxMembers.Value;
                })
                .WithMessage("MinMembers cannot be greater than MaxMembers when both provided.");

            // Validate Status enum
            RuleFor(x => x.Status)
                .Must(s => s == null || Enum.TryParse(typeof(ProjectStatus), s, true, out _))
                .WithMessage("Invalid project status value.");

            RuleFor(x => x.MentorId)
                .GreaterThan(0).When(x => x.MentorId.HasValue)
                .WithMessage("MentorId must be greater than 0 if provided.");
        }
    }
}
