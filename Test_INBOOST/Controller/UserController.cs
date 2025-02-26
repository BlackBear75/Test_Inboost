using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Models.UsersModel;
using Test_INBOOST.Service;

namespace Test_INBOOST.Controller;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserResponce createUser)
    {
        await _userService.CreateUser(createUser);
        return Ok("Користувач доданий у базу.");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (await _userService.DeleteUser(id))
        {
            return Ok("Користувач видалений.");
        }

        return NotFound("Користувач не знайдений ");
    }
    
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok( await  _userService.GetAllUsers());
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUserResponce createUser)
    {
        if (await _userService.UpdateUser(id, createUser))
        {
            return Ok("Інформація про користувача оновлена.");
        }

        return NotFound("Користувач не знайдений.");

    }
}
