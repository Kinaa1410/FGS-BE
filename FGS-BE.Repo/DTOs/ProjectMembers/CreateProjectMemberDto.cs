

namespace FGS_BE.Repo.DTOs.ProjectMembers
{
    public class CreateProjectMemberDto
    {
        public string? Role { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

        public FGS_BE.Repo.Entities.ProjectMember ToEntity()
        {
            return new FGS_BE.Repo.Entities.ProjectMember
            {
                Role = Role,
                UserId = UserId,
                ProjectId = ProjectId,
                JoinAt = DateTime.UtcNow
            };
        }
    }
}
