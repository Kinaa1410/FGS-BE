using FGS_BE.Repo.Entities;
using Microsoft.AspNetCore.Http;

namespace FGS_BE.Repo.DTOs.Submissions
{
    public class CreateSubmissionDto
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public IFormFile? File { get; set; } = default!;
    }
}
