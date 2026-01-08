using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Resources;
using FGS_BE.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace FGS_BE.Service.Services;

public class UserService(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    IJwtService jwtService) : IUserService
{
    private readonly IGenericRepository<User> _userRepository = unitOfWork.Repository<User>();
    private readonly IEmailService _emailService;

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

    public async Task<AccessTokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
            return null;

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            return null;

        return await jwtService.GenerateTokenAsync(user);
    }

    public async Task<VerifyResponse> Verify(string token)
    {
        var verify = await _userRepository.Verify(token);

        if (verify == null)
        {
            return new VerifyResponse
            {
                Success = false,
                Message = "Invalid or expired token"
            };
        }

        verify.Status = "Active";
        verify.VerificationToken = null; // xoá token sau khi verify
        await _userRepository.UpdateAsync(verify);

        return new VerifyResponse
        {
            Success = true,
            Message = "Account verified successfully"
        };
    }

    public async Task<User> RegisterCustomer(RegisterDTO customer)
    {
        var exitMail = await _userRepository.FindByAsync(x => x.Email == customer.Email);

        if (exitMail != null)
        {
            throw new Exception("Email already exists");
        }


        var token = _emailService.GenerateRandomNumber();

        var user = new User
        {
            UserName = customer.Username,
            Email = customer.Email,
            FullName = customer.FullName ?? string.Empty,
            StudentCode = customer.StudentCode ?? string.Empty,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _emailService.SendOtpMail(customer.FullName, token, customer.Email);

        var result = await userManager.CreateAsync(user, customer.Password);
        if (!result.Succeeded) throw new BadRequestException();

        result = await userManager.AddToRoleAsync(user, RoleEnums.User.ToString());
        if (!result.Succeeded) throw new BadRequestException();

        // New: Create UserWallet with balance=0
        var walletRepo = unitOfWork.Repository<UserWallet>();
        var wallet = new UserWallet
        {
            UserId = user.Id,
            Balance = 0, // Starting balance (matches entity property)
            LastUpdatedAt = DateTime.UtcNow // Matches entity property
        };
        await walletRepo.CreateAsync(wallet);

        // New: Auto-assign lowest level (dynamic query for smallest threshold)
        var levelRepo = unitOfWork.Repository<Level>();
        var userLevelRepo = unitOfWork.Repository<UserLevel>();

        // Fixed: Load active levels to memory first, then sort client-side to avoid EF translation error
        var activeLevels = await levelRepo.Entities
            .Where(l => l.IsActive)
            .ToListAsync();

        var lowestLevel = activeLevels
            .OrderBy(l => GetThresholdFromCondition(l.ConditionJson)) // Now safe in memory
            .FirstOrDefault();

        if (lowestLevel != null)
        {
            var initialUserLevel = new UserLevel
            {
                UserId = user.Id,
                LevelId = lowestLevel.Id,
                UnlockedAt = DateTime.UtcNow
            };
            await userLevelRepo.CreateAsync(initialUserLevel);
        }

        // Commit all changes atomically
        await unitOfWork.CommitAsync();

        return user;
    }


    public async Task RegisterAsync(RegisterRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is not null) throw new BadRequestException(Resource.UsernameExisted);

        user = new User
        {
            UserName = request.Username,
            Email = request.Username,
            FullName = request.FullName ?? string.Empty, 
            StudentCode = request.StudentCode ?? string.Empty,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException();

        result = await userManager.AddToRoleAsync(user, RoleEnums.User.ToString());
        if (!result.Succeeded) throw new BadRequestException();

        // New: Create UserWallet with balance=0
        var walletRepo = unitOfWork.Repository<UserWallet>();
        var wallet = new UserWallet
        {
            UserId = user.Id,
            Balance = 0, // Starting balance (matches entity property)
            LastUpdatedAt = DateTime.UtcNow // Matches entity property
        };
        await walletRepo.CreateAsync(wallet);

        // New: Auto-assign lowest level (dynamic query for smallest threshold)
        var levelRepo = unitOfWork.Repository<Level>();
        var userLevelRepo = unitOfWork.Repository<UserLevel>();

        // Fixed: Load active levels to memory first, then sort client-side to avoid EF translation error
        var activeLevels = await levelRepo.Entities
            .Where(l => l.IsActive)
            .ToListAsync();

        var lowestLevel = activeLevels
            .OrderBy(l => GetThresholdFromCondition(l.ConditionJson)) // Now safe in memory
            .FirstOrDefault();

        if (lowestLevel != null)
        {
            var initialUserLevel = new UserLevel
            {
                UserId = user.Id,
                LevelId = lowestLevel.Id,
                UnlockedAt = DateTime.UtcNow
            };
            await userLevelRepo.CreateAsync(initialUserLevel);
        }

        // Commit all changes atomically
        await unitOfWork.CommitAsync();
    }

    // Private helper method (add to UserService class)
    private int GetThresholdFromCondition(string? conditionJson)
    {
        if (string.IsNullOrEmpty(conditionJson)) return int.MaxValue; // High value if no condition

        try
        {
            using var condition = JsonDocument.Parse(conditionJson);
            return condition.RootElement.TryGetProperty("threshold", out var thresholdProp) &&
                   thresholdProp.ValueKind == JsonValueKind.Number
                ? thresholdProp.GetInt32()
                : int.MaxValue;
        }
        catch (JsonException)
        {
            return int.MaxValue;
        }
    }

    public async Task<AccessTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (user == null) throw new UnauthorizedAccessException(Resource.InvalidRefreshToken);
        await userManager.UpdateSecurityStampAsync(user);
        return await jwtService.GenerateTokenAsync(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException(errors);
        }

        await userManager.UpdateSecurityStampAsync(user);
    }

    public async Task RegisterMentorAsync(RegisterStaffRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is not null) throw new BadRequestException(Resource.UsernameExisted);

        user = new User
        {
            UserName = request.Username,
            Email = request.Username,
            FullName = request.FullName ?? string.Empty,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException();

        result = await userManager.AddToRoleAsync(user, RoleEnums.Mentor.ToString());
        if (!result.Succeeded) throw new BadRequestException();

        var walletRepo = unitOfWork.Repository<UserWallet>();
        var wallet = new UserWallet
        {
            UserId = user.Id,
            Balance = 0, 
            LastUpdatedAt = DateTime.UtcNow 
        };
        await walletRepo.CreateAsync(wallet);

        var levelRepo = unitOfWork.Repository<Level>();
        var userLevelRepo = unitOfWork.Repository<UserLevel>();

        var activeLevels = await levelRepo.Entities
            .Where(l => l.IsActive)
            .ToListAsync();

        var lowestLevel = activeLevels
            .OrderBy(l => GetThresholdFromCondition(l.ConditionJson)) 
            .FirstOrDefault();

        if (lowestLevel != null)
        {
            var initialUserLevel = new UserLevel
            {
                UserId = user.Id,
                LevelId = lowestLevel.Id,
                UnlockedAt = DateTime.UtcNow
            };
            await userLevelRepo.CreateAsync(initialUserLevel);
        }

        await unitOfWork.CommitAsync();
    }

    public async Task RegisterFinanceAsync(RegisterStaffRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is not null) throw new BadRequestException(Resource.UsernameExisted);

        user = new User
        {
            UserName = request.Username,
            Email = request.Username,
            FullName = request.FullName ?? string.Empty,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException();

        result = await userManager.AddToRoleAsync(user, RoleEnums.FinanceOfficer.ToString());
        if (!result.Succeeded) throw new BadRequestException();

        var walletRepo = unitOfWork.Repository<UserWallet>();
        var wallet = new UserWallet
        {
            UserId = user.Id,
            Balance = 0,
            LastUpdatedAt = DateTime.UtcNow
        };
        await walletRepo.CreateAsync(wallet);

        var levelRepo = unitOfWork.Repository<Level>();
        var userLevelRepo = unitOfWork.Repository<UserLevel>();

        var activeLevels = await levelRepo.Entities
            .Where(l => l.IsActive)
            .ToListAsync();

        var lowestLevel = activeLevels
            .OrderBy(l => GetThresholdFromCondition(l.ConditionJson))
            .FirstOrDefault();

        if (lowestLevel != null)
        {
            var initialUserLevel = new UserLevel
            {
                UserId = user.Id,
                LevelId = lowestLevel.Id,
                UnlockedAt = DateTime.UtcNow
            };
            await userLevelRepo.CreateAsync(initialUserLevel);
        }

        await unitOfWork.CommitAsync();
    }

}