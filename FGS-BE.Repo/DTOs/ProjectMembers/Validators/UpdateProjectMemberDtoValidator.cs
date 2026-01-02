using FluentValidation;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.ProjectMembers.Validators
{
    public class UpdateProjectMemberDtoValidator : AbstractValidator<UpdateProjectMemberDto>
    {
        public UpdateProjectMemberDtoValidator()
        {
            RuleFor(x => x.Role)
                .Must(BeAValidRole)
                .WithMessage("Role must be a valid value from RoleEnums (Admin, User, Staff, Mentor, FinanceOfficer).")
                .When(x => !string.IsNullOrWhiteSpace(x.Role));
        }

        private static bool BeAValidRole(string role)
        {
            return Enum.TryParse<RoleEnums>(role, true, out _);
        }
    }
}