using FluentValidation;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.ProjectMembers.Validators
{
    public class CreateProjectMemberDtoValidator : AbstractValidator<CreateProjectMemberDto>
    {
        public CreateProjectMemberDtoValidator()
        {
            RuleFor(x => x.Role)
                .Must(BeAValidRole)
                .WithMessage("Role must be a valid value from RoleEnums (Admin, User, Staff, Mentor, FinanceOfficer).")
                .When(x => !string.IsNullOrWhiteSpace(x.Role));

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("ProjectId must be greater than 0.");
        }

        private static bool BeAValidRole(string role)
        {
            return Enum.TryParse<RoleEnums>(role, true, out _);
        }
    }
}