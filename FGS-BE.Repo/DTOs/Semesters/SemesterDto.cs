

namespace FGS_BE.Repo.DTOs.Semesters
{
    public class SemesterDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? KeywordTheme { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public SemesterDto() { }

        public SemesterDto(FGS_BE.Repo.Entities.Semester entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            KeywordTheme = entity.KeywordTheme;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Status = entity.Status;
            CreatedAt = entity.CreatedAt;
        }
    }
}
