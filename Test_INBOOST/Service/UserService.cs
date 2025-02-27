using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Models.UsersModel;
using Test_INBOOST.Models.WeatherHistoryModel;

namespace Test_INBOOST.Service;

public interface IUserService
{
    Task<bool> CreateUser([FromBody] CreateUserResponce createUser);
    Task<bool> DeleteUser(Guid id);
    Task<List<GetUserResponce>> GetAllUsers();

    Task<List<GetUserAndWeatherHistoryResponse>> GetUserAndWeatherHistory(Guid id,long userId);
    Task<GetUserResponce> GetUserById(Guid id);
    Task<bool> UpdateUser (Guid id, [FromBody] CreateUserResponce createUser);
}

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;
    private readonly IWeatherHistoryService _weatherHistoryService;

    public UserService(IUserRepository<User> userRepository, IWeatherHistoryService weatherHistoryService)
    {
        _weatherHistoryService = weatherHistoryService;
        _userRepository = userRepository;   
    }

    public async Task<bool> CreateUser(CreateUserResponce createUser)
    {
        try
        {
            User newUser = new User()
            {
                UserName = createUser.UserName,
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
                Id = user.Id,
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }

        return result;
    }

    public async Task<List<GetUserAndWeatherHistoryResponse>> GetUserAndWeatherHistory(Guid id,long userId = 0)
    {

        var existingUser = new User();
        
        if (userId == 0)
        {
             existingUser  = await _userRepository.FindByIdAsync(id);
        }
        else
        {
            existingUser = await _userRepository.FindByUserIdAsync(userId);
        }

        var listWeatherHistory =await _weatherHistoryService.GetAllUserWeatherHistory(existingUser.UserId);

        var resultWeatherHistory = new List<GetUserAndWeatherHistoryResponse>();
        foreach (var item in listWeatherHistory)
        {
            if(item.RecipientUserId!=null) continue;
            
             var getUserAndWeatherHistory = new GetUserAndWeatherHistoryResponse()
            {
                UserId = existingUser.UserId,
                UserName = existingUser.UserName,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                WeatherHistory = new List<WeatherHistory>()
            };
             getUserAndWeatherHistory.WeatherHistory.Add(new WeatherHistory()
             {
                 Id = item.Id,
                 Country = item.Country,
                 City = item.City,
                 Humidity = item.Humidity,
                 Temperature = item.Temperature,
                 WindSpeed = item.WindSpeed,
                 FeelsLike = item.FeelsLike,
                 WeatherDescription = item.WeatherDescription,
                 CreationDate = item.CreationDate
             });
             
             resultWeatherHistory.Add(getUserAndWeatherHistory);
        }


        return resultWeatherHistory;
    }

    public async Task<GetUserResponce> GetUserById(Guid id)
    {
        var existingUser  = await _userRepository.FindByIdAsync(id);
        if (existingUser  == null) return null;
        return new GetUserResponce()
        {
            Id = existingUser.Id,
            UserId = existingUser.UserId,
            UserName = existingUser.UserName,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
        };
    }
    

    public async Task<bool> UpdateUser(Guid id, CreateUserResponce createUser)
    {
        var existingUser  = await _userRepository.FindByIdAsync(id);
        if (existingUser  == null) return false;

        existingUser .UserName = createUser.UserName;
        existingUser .FirstName = createUser.FirstName;
        existingUser .LastName = createUser.LastName;

        await _userRepository.UpdateOneAsync(existingUser );
        return true;
    }
}