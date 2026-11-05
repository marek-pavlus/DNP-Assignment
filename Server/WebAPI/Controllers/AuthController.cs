using Microsoft.AspNetCore.Mvc;
using ApiContracts;
using ApiContracts.Users; 
using ApiContracts.Auth;
using RepositoryContracts; 

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        var user = userRepository.GetManyUsers().SingleOrDefault(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password.");
        }

        UserDto userDto = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username
        };

        return Ok(userDto);
    }
}