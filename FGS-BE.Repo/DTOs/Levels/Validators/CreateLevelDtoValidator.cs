using FluentValidation;
using System.Text.Json;

namespace FGS_BE.Repo.DTOs.Levels.Validators
{
    public class CreateLevelDtoValidator : AbstractValidator<CreateLevelDto>
    {
        public CreateLevelDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Level name cannot be empty!")
                .Length(1, 100).WithMessage("Level name must be between 1-100 characters!");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("Description cannot exceed 100 characters!");

            RuleFor(x => x.ConditionJson)
                .NotEmpty().WithMessage("Condition JSON cannot be empty!")
                .Must(BeValidConditionJson).WithMessage("Condition JSON must be valid (valid JSON with 'threshold' as a number >= 0)!");

            RuleFor(x => x.PointsReward)
                .GreaterThanOrEqualTo(0).WithMessage("Points reward must be >= 0!");


            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("Active status must be specified!");
        }

        private static bool BeValidConditionJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
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