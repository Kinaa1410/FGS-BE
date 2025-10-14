using FGS_BE.Repo.DTOs.Achievements;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Services.Interfaces;
public interface IAchievementService
{
    Task<PaginatedResponse<AchievementResponse>> FindAsync(GetAchievementsQuery request);
    Task<AchievementResponse> FindByAsync(int id);
    Task UpdateAsync(int id, UpdateAchievementCommand request);
    Task CreateAsync(CreateAchievementCommand command);
    Task DeleteAsync(int id);
}
