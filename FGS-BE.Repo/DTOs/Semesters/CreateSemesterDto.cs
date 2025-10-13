using System;
using System.Collections.Generic;


namespace FGS_BE.Repo.DTOs.Semesters
{
    public class CreateSemesterDto
    {
        public string Name { get; set; } = string.Empty;
        public string? KeywordTheme { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Upcoming";

        public Entities.Semester ToEntity()
        {
            return new Entities.Semester
            {
                Name = Name,
                KeywordTheme = KeywordTheme,
                StartDate = StartDate,
                EndDate = EndDate,
                Status = Status,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}


