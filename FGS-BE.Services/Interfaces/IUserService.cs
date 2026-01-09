using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Repo.Entities;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Task = System.Threading.Tasks.Task;

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
    Task RegisterMentorAsync(RegisterStaffRequest request);
    Task RegisterFinanceAsync(RegisterStaffRequest request);

    Task<User> RegisterMailAcc(RegisterDTO customer);
    Task<VerifyResponse> Verify(string token);
}
