using SchedulePlanner.Application.DTO;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<Dto.UserDto> CreateUserAsync(Dto.CreateUserDto dto)
    {
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Username = dto.Username, 
            Email = dto.Email 
        };
            
        await _userRepo.AddAsync(user);
        return new Dto.UserDto(user.Id, user.Username, user.Email);
    }

    public async Task<List<Dto.UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => new Dto.UserDto(u.Id, u.Username, u.Email)).ToList();
    }
}