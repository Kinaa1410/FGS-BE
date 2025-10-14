using FGS_BE.Repo.DTOs.Achievements;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;
using Task = System.Threading.Tasks.Task;

namespace FGS_BE.Services.Services;
public class AchievementService(IUnitOfWork unitOfWork) : IAchievementService
{
    private readonly IGenericRepository<Achievement> _achievementRepository = unitOfWork.Repository<Achievement>();

    public async Task<PaginatedResponse<AchievementResponse>> FindAsync(GetAchievementsQuery request)
    {
        var entities = await _achievementRepository
            .FindAsync<AchievementResponse>(
                request.PageIndex,
                request.PageSize,
                request.GetExpressions(),
                request.GetOrder());
        return await entities.ToPaginatedResponseAsync();
    }

    public async Task<AchievementResponse> FindByAsync(int id)
    {
        var entity = await _achievementRepository.FindByAsync<AchievementResponse>(x => x.Id == id);
        if (entity == null) throw new NotFoundException(nameof(Achievement), id);
        return entity;
    }

    public async Task CreateAsync(CreateAchievementCommand command)
    {
        var entity = command.Adapt<Achievement>();
        await _achievementRepository.CreateAsync(entity);
        await unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdateAchievementCommand request)
    {
        var entity = await _achievementRepository.FindByAsync(x => x.Id == id);
        if (entity is null) throw new NotFoundException(nameof(Achievement), id);
        request.Adapt(entity);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _achievementRepository.FindByAsync(x => x.Id == id);
        if (entity is null) throw new NotFoundException(nameof(Achievement), id);
        await _achievementRepository.DeleteAsync(entity);
        await unitOfWork.CommitAsync();
    }

}
