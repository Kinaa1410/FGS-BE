using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;

namespace FGS_BE.Services.Interfaces;
public interface IUserService
{
    Task<UserResponse> LoginAsync(LoginRequest request);
    Task RegisterAsync(RegisterRequest request);
    Task<PaginatedResponse<UserResponse>> FindAsync(GetUsersQuery request);
    Task<UserResponse> FindByAsync(int id);
    Task UpdateAsync(int id, UpdateUserCommand request);
    Task DeleteAsync(int id);
}
