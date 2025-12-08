using FluentValidation;
using System.Text.Json;

namespace FGS_BE.Repo.DTOs.Levels.Validators
{
    public class UpdateLevelDtoValidator : AbstractValidator<UpdateLevelDto>
    {
        public UpdateLevelDtoValidator()
        {
            // Partial, nên optional
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Level name cannot exceed 100 characters!")
                .NotEmpty().When(x => x.Name != null).WithMessage("Level name cannot be empty if provided!");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("Description cannot exceed 100 characters!");

            RuleFor(x => x.ConditionJson)
                .Must(BeValidConditionJson).When(x => !string.IsNullOrEmpty(x.ConditionJson))
                .WithMessage("Condition JSON must be valid if provided!");

            RuleFor(x => x.PointsReward)
                .GreaterThanOrEqualTo(0).When(x => x.PointsReward.HasValue)
                .WithMessage("Points reward must be >= 0 if provided!");


            // IsActive optional, no rule
        }

        private static bool BeValidConditionJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return true; // Optional
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty("threshold", out var threshold) &&
                       threshold.ValueKind == JsonValueKind.Number &&
                       threshold.GetInt32() >= 0;
            }
            catch (JsonException)
            {
                return false;
            }
        }

    }
}