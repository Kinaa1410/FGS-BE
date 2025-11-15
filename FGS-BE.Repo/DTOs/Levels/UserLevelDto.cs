using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Levels;

public class UserLevelDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LevelId { get; set; }
    public DateTime UnlockedAt { get; set; }
    public LevelDto Level { get; set; } = null!;

    public UserLevelDto() { }

    public UserLevelDto(FGS_BE.Repo.Entities.UserLevel entity)
    {
        Id = entity.Id;
        UserId = entity.UserId;
        LevelId = entity.LevelId;
        UnlockedAt = entity.UnlockedAt;
        Level = new LevelDto(entity.Level);
    }
}