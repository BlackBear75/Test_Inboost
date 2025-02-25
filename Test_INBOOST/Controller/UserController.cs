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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserResponce createUser)
    {
        User newUser = new User()
        {
            UserName = createUser.UserName,
            Email = createUser.Email,
            FirstName = createUser.FirstName,
            LastName = createUser.LastName,
            Role = UserRole.User,
        };
        
        await _userRepository.InsertOneAsync(newUser);
        return Ok("Користувач доданий у базу.");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) return NotFound("Користувач не знайдений.");

        await _userRepository.DeleteOneAsync(id);
        return Ok("Користувач видалений.");
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();

        var result = new List<GetUserResponce>();

        foreach (var user in users)
        {
            result.Add(new GetUserResponce()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }
        
        return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser (Guid id, [FromBody] CreateUserResponce createUser)
    {
        var existingUser  = await _userRepository.FindByIdAsync(id);
        if (existingUser  == null) return NotFound("Користувач не знайдений.");

        existingUser .UserName = createUser.UserName;
        existingUser .Email = createUser.Email;
        existingUser .FirstName = createUser.FirstName;
        existingUser .LastName = createUser.LastName;

        await _userRepository.UpdateOneAsync(existingUser );
        return Ok("Інформація про користувача оновлена.");
    }
   
}
