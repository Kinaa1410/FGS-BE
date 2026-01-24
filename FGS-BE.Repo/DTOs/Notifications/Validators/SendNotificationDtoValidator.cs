using FluentValidation;

namespace FGS_BE.Repo.DTOs.Notifications.Validators
{
    public class SendNotificationDtoValidator : AbstractValidator<SendNotificationDto>
    {
        public SendNotificationDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0!");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("Template code cannot be empty!")
                .MaximumLength(50).WithMessage("Template code cannot exceed 50 characters!");

            RuleFor(x => x.Placeholders)
                .Must(BeValidPlaceholders).When(x => x.Placeholders != null)
                .WithMessage("Placeholders must not contain null keys or exceed 10 entries!");

        }

        private static bool BeValidPlaceholders(Dictionary<string, object>? placeholders)
        {
            if (placeholders == null || placeholders.Count == 0) return true;
            if (placeholders.Count > 10) return false; // Limit size to avoid abuse
            return placeholders.Keys.All(k => !string.IsNullOrWhiteSpace(k)) && // No null/empty keys
                   placeholders.Values.All(v => v != null); // No null values
        }
    }
}