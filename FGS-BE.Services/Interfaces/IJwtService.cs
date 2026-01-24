using FGS_BE.Repo.Entities;
using Microsoft.AspNetCore.Authentication.BearerToken;

namespace FGS_BE.Services.Interfaces;
public interface IJwtService
{
    public Task<AccessTokenResponse> GenerateTokenAsync(User user, long? expiresTime = null);
    public Task<User> ValidateRefreshTokenAsync(string refreshToken);

}
