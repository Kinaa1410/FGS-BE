using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Task = System.Threading.Tasks.Task;

namespace FGS_BE.Services.Services;
public class UserService(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager) : IUserService
{
    private readonly IGenericRepository<User> _userRepository = unitOfWork.Repository<User>();

    public async Task<PaginatedResponse<UserResponse>> FindAsync(GetUsersQuery request)
    {
        var entities = await _userRepository
            .FindAsync<UserResponse>(
                request.PageIndex,
                request.PageSize,
                request.GetExpressions(),
                request.GetOrder());
        return await entities.ToPaginatedResponseAsync();
    }

    public async Task<UserResponse> FindByAsync(int id)
    {
        var entity = await _userRepository.FindByAsync<UserResponse>(x => x.Id == id);
        if (entity == null) throw new NotFoundException(nameof(User), id);
        return entity;
    }

    public async Task UpdateAsync(int id, UpdateUserCommand request)
    {
        var entity = await _userRepository.FindByAsync(x => x.Id == id);
        if (entity is null) throw new NotFoundException(nameof(User), id);
        request.Adapt(entity);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _userRepository.FindByAsync(x => x.Id == id);
        if (entity is null) throw new NotFoundException(nameof(User), id);
        await _userRepository.DeleteAsync(entity);
        await unitOfWork.CommitAsync();
    }

    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Unauthorized");
        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Unauthorized");
        return user.Adapt<UserResponse>();
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is not null) throw new BadRequestException("Username đã tồn tại");
        user = new User
        {
            UserName = request.Username,
            Email = request.Username,
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException();
        result = await userManager.AddToRoleAsync(user, RoleEnums.User.ToString());
        if (!result.Succeeded) throw new BadRequestException();

    }
}
