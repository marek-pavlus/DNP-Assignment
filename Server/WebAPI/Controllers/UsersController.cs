using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        User? existingUser = userRepository
            .GetManyUsers()
            .FirstOrDefault(user => user.Username == request.Username);

        if (existingUser is not null)
        {
            return BadRequest("Username already exists.");
        }

        User user = new User(request.Username, request.Password);

        User createdUser = await userRepository.AddUserAsync(user);

        UserDto userDto = new UserDto
        {
            UserId = createdUser.UserId,
            Username = createdUser.Username
        };

        return CreatedAtAction(
            nameof(GetSingleUser),
            new { id = createdUser.UserId },
            userDto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingleUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepository.GetSingleUserAsync(id);

            UserDto userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };

            return Ok(userDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID '{id}' not found.");
        }
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetManyUsers(
        [FromQuery] string? username)
    {
        IQueryable<User> users = userRepository.GetManyUsers();

        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(user =>
                user.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
        }

        IEnumerable<UserDto> userDtos = users.Select(user => new UserDto
        {
            UserId = user.UserId,
            Username = user.Username
        });

        return Ok(userDtos);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserDto request)
    {
        try
        {
            User user = await userRepository.GetSingleUserAsync(id);

            User? existingUser = userRepository
                .GetManyUsers()
                .FirstOrDefault(u =>
                    u.Username == request.Username &&
                    u.UserId != id);

            if (existingUser is not null)
            {
                return BadRequest("Username already exists.");
            }
            
            user.Username = request.Username;
            user.Password = request.Password;

            await userRepository.UpdateUserAsync(user);

            UserDto userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };

            return Ok(userDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID '{id}' not found.");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepository.DeleteUserAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID '{id}' not found.");
        }
    }
}