namespace FGS_BE.Repo.DTOs.ProjectMembers
{
    public class ProjectMemberDto
    {
        public int Id { get; set; }
        public string? Role { get; set; }
        public DateTime JoinAt { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

        public ProjectMemberDto() { }

        public ProjectMemberDto(FGS_BE.Repo.Entities.ProjectMember entity)
        {
            Id = entity.Id;
            Role = entity.Role;
            JoinAt = entity.JoinAt;
            UserId = entity.UserId;
            ProjectId = entity.ProjectId;
        }
    }
}