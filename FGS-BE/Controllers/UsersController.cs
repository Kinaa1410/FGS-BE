using FGS_BE.Exceptions;
using FGS_BE.Repo.DTOs;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository = unitOfWork.Repository<User>();

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<UserResponse>>> GetUsers([FromQuery] GetUsersQuery request)
    {
        var users = await _userRepository
            .FindAsync<UserResponse>(
                request.PageIndex,
                request.PageSize,
                request.GetExpressions(),
                request.GetOrder());

        return await users.ToPaginatedResponseAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var user = await _userRepository
            .FindByAsync<UserResponse>(x => x.Id == id);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        return user;
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponse>> CreateUser(CreateUserCommand command)
    {

        var entity = command.Adapt<User>();

        await _userRepository.CreateAsync(entity);
        await unitOfWork.CommitAsync();

        return new MessageResponse("Created Success");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MessageResponse>> UpdateUser(int id, UpdateUserCommand request)
    {
        var user = await _userRepository.FindByAsync(x => x.Id == id);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), id);
        }
        request.Adapt(user);
        await unitOfWork.CommitAsync();
        return new MessageResponse("Updated Success");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageResponse>> DeleteUser(int id)
    {
        var user = await _userRepository.FindByAsync(x => x.Id == id);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        await _userRepository.DeleteAsync(user);
        await unitOfWork.CommitAsync();

        return new MessageResponse("Deleted Success");
    }
}
