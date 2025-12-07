using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using Microsoft.AspNetCore.Authentication.BearerToken;

namespace FGS_BE.Services.Interfaces;
public interface IUserService
{
    Task<AccessTokenResponse> LoginAsync(LoginRequest request);
    Task<AccessTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task RegisterAsync(RegisterRequest request);
    Task<PaginatedResponse<UserResponse>> FindAsync(GetUsersQuery request);
    Task<UserResponse> FindByAsync(int id);
    Task UpdateAsync(int id, UpdateUserCommand request);
    Task DeleteAsync(int id);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task RegisterMentorAsync(RegisterRequest request);
    Task RegisterFinanceAsync(RegisterRequest request);
}
