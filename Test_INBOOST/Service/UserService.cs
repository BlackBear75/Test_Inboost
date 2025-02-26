using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Models.UsersModel;

namespace Test_INBOOST.Service;

public interface IUserService
{
    Task<bool> CreateUser([FromBody] CreateUserResponce createUser);
    Task<bool> DeleteUser(Guid id);
    Task<List<GetUserResponce>> GetAllUsers();
    Task<bool> UpdateUser (Guid id, [FromBody] CreateUserResponce createUser);
}

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;

    public UserService(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository;   
    }

    public async Task<bool> CreateUser(CreateUserResponce createUser)
    {
        try
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
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteOneAsync(id);
        return true;
    }

    public async Task<List<GetUserResponce>> GetAllUsers()
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

        return result;
    }

    public async Task<bool> UpdateUser(Guid id, CreateUserResponce createUser)
    {
        var existingUser  = await _userRepository.FindByIdAsync(id);
        if (existingUser  == null) return false;

        existingUser .UserName = createUser.UserName;
        existingUser .Email = createUser.Email;
        existingUser .FirstName = createUser.FirstName;
        existingUser .LastName = createUser.LastName;

        await _userRepository.UpdateOneAsync(existingUser );
        return true;
    }
}