using FluentValidation;

namespace FGS_BE.Repo.DTOs.Notifications.Validators
{
    public class CreateNotificationTemplateDtoValidator : AbstractValidator<CreateNotificationTemplateDto>
    {
        public CreateNotificationTemplateDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code cannot be empty!")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters!");

            RuleFor(x => x.SubjectTemplate)
                .NotEmpty().WithMessage("Subject template cannot be empty!")
                .MaximumLength(500).WithMessage("Subject template cannot exceed 500 characters!");

            RuleFor(x => x.BodyTemplate)
                .NotEmpty().WithMessage("Body template cannot be empty!")
                .MaximumLength(5000).WithMessage("Body template cannot exceed 5000 characters!");
        }
    }

    public class UpdateNotificationTemplateDtoValidator : AbstractValidator<UpdateNotificationTemplateDto>
    {
        public UpdateNotificationTemplateDtoValidator()
        {
            RuleFor(x => x.Code)
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters!")
                .NotEmpty().When(x => x.Code != null).WithMessage("Code cannot be empty if provided!");

            RuleFor(x => x.SubjectTemplate)
                .MaximumLength(500).WithMessage("Subject template cannot exceed 500 characters!")
                .NotEmpty().When(x => x.SubjectTemplate != null).WithMessage("Subject template cannot be empty if provided!");

            RuleFor(x => x.BodyTemplate)
                .MaximumLength(5000).WithMessage("Body template cannot exceed 5000 characters!")
                .NotEmpty().When(x => x.BodyTemplate != null).WithMessage("Body template cannot be empty if provided!");

            // IsActive optional
        }
    }
}