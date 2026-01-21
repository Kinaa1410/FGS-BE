namespace FGS_BE.Repo.DTOs.Semesters
{
    public class SemesterDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? KeywordTheme { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }

        public SemesterDto() { }

        public SemesterDto(FGS_BE.Repo.Entities.Semester entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Id = entity.Id;
            Name = entity.Name;
            KeywordTheme = entity.KeywordTheme;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            CreatedAt = entity.CreatedAt;

            var now = DateTime.UtcNow;
            if (now < entity.StartDate)
                Status = "Upcoming";
            else if (now > entity.EndDate)
                Status = "Closed";
            else
                Status = "Active";
        }
    }
}