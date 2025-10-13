

namespace FGS_BE.Repo.DTOs.Semesters
{
    public class UpdateSemesterDto
    {
        public string? Name { get; set; }
        public string? KeywordTheme { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }

        public void ApplyToEntity(Entities.Semester entity)
        {
            if (Name != null) entity.Name = Name;
            if (KeywordTheme != null) entity.KeywordTheme = KeywordTheme;
            if (StartDate.HasValue) entity.StartDate = StartDate.Value;
            if (EndDate.HasValue) entity.EndDate = EndDate.Value;
            if (Status != null) entity.Status = Status;
        }
    }
}

