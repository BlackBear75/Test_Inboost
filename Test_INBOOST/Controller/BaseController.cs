using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Models.UsersModel;

namespace Test_INBOOST.Controller;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository<User> _userRepository;

    public UserController(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
    {
        User newUser = new User()
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = UserRole.User,
        };
        
        await _userRepository.InsertOneAsync(newUser);
        return Ok("Користувач доданий у базу.");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser (long id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) return NotFound("Користувач не знайдений.");

        await _userRepository.DeleteOneAsync(id);
        return Ok("Користувач видалений.");
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(long id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) return NotFound("Користувач не знайдений.");
        return Ok(user);
    }
}
