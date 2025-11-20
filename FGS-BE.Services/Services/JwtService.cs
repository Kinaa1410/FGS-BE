using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Resources;
using FGS_BE.Repo.Settings;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FGS_BE.Service.Services;
public class JwtService : IJwtService
{

    private readonly JwtSettings _jwtSettings;
    private readonly SignInManager<User> _signInManager;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly IDataProtector protector;
    private readonly TicketDataFormat ticketDataFormat;

    public JwtService(
        SignInManager<User> signInManager,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        _dataProtectionProvider = dataProtectionProvider;

        protector = _dataProtectionProvider.CreateProtector(_jwtSettings.SerectRefreshKey);
        ticketDataFormat = new TicketDataFormat(protector);
    }

    public async Task<AccessTokenResponse> GenerateTokenAsync(User user, long? expiresTime = null)
    {
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        var claims = claimsPrincipal.Claims.ToList();
        claims.Add(new Claim(ClaimUsers.FullName, user.FullName ?? ""));
        claims.Add(new Claim(ClaimUsers.PhoneNumber, user.PhoneNumber ?? ""));
        claims.Add(new Claim(ClaimUsers.UserName, user.UserName ?? ""));
        claims.Add(new Claim(ClaimUsers.Id, user.Id.ToString() ?? ""));
        claims.Add(new Claim(ClaimUsers.Email, user.Email ?? ""));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SerectKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var expires = expiresTime ?? _jwtSettings.TokenExpire;
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expires),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new AccessTokenResponse
        {
            AccessToken = jwt,
            ExpiresIn = expires,
            RefreshToken = ticketDataFormat.Protect(CreateRefreshTicket(claimsPrincipal, DateTimeOffset.UtcNow)),
        };
        return response;
    }

    public async Task<User> ValidateRefreshTokenAsync(string refreshToken)
    {
        var ticket = ticketDataFormat.Unprotect(refreshToken);
        if (ticket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            DateTimeOffset.UtcNow >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(ticket.Principal) is not User user)
            throw new UnauthorizedAccessException(Resource.InvalidRefreshToken);
        return user;
    }

    private AuthenticationTicket CreateRefreshTicket(ClaimsPrincipal user, DateTimeOffset utcNow)
    {
        var refreshProperties = new AuthenticationProperties
        {
            ExpiresUtc = utcNow.AddSeconds(_jwtSettings.RefreshTokenExpire)
        };
        return new AuthenticationTicket(user, refreshProperties, JwtBearerDefaults.AuthenticationScheme);
    }
}

public struct ClaimUsers
{
    public const string Id = "id";
    public const string Email = "email";
    public const string FullName = "full_name";
    public const string PhoneNumber = "phone_number";
    public const string UserName = "user_name";
}