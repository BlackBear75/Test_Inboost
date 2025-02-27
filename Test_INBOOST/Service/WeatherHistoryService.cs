using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;
using Test_INBOOST.Models.UsersModel;
using Test_INBOOST.Models.WeatherHistoryModel;
using User = Test_INBOOST.Entity.User.User;

namespace Test_INBOOST.Service;

public interface IWeatherHistoryService
{
    Task<List<WeatherHistory>> GetAllUserWeatherHistory(long userId);
    Task<bool> CreateWeatherHistory(CreateWeatherHistoryResponce createWeatherHistoryResponce);

    Task<WeatherHistory> GetWeatherHistoryById(Guid id);
    
    Task<List<WeatherHistory>> GetReceivedWeatherHistory(long id);

    Task<bool> UpdateWeatherHistory(Guid id,
        [FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce);

    Task<bool> DeleteWeatherHistory(Guid id);
}

public class WeatherHistoryService : IWeatherHistoryService
{
    private readonly IWeatherHistoryRepository<WeatherHistory> _weatherHistoryRepository;

    private readonly IUserRepository<User> _userRepository;

    public WeatherHistoryService(IWeatherHistoryRepository<WeatherHistory> weatherHistoryRepository,IUserRepository<User> userRepository)
    {
        _weatherHistoryRepository = weatherHistoryRepository;
        _userRepository = userRepository;
     
    }

    public async Task<List<WeatherHistory>> GetAllUserWeatherHistory(long userId)
    {
        
        var weatherHistoryList = await _weatherHistoryRepository.FindByUserIdAsync(userId);
        
        return weatherHistoryList.ToList();
    }

    public async Task<bool> CreateWeatherHistory(CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        
        try
        {
            var existingUser  = await _userRepository.FindByUserIdAsync(createWeatherHistoryResponce.UserId);
            if (existingUser == null)
            {
                return false;
            }
            
            WeatherHistory newWeatherHistory = new WeatherHistory()
            {
                UserId = createWeatherHistoryResponce.UserId,
                City = createWeatherHistoryResponce.City,
                WeatherDescription = createWeatherHistoryResponce.WeatherDescription,
                Temperature = createWeatherHistoryResponce.Temperature,
                FeelsLike = createWeatherHistoryResponce.FeelsLike,
                Humidity = createWeatherHistoryResponce.Humidity,
                WindSpeed = createWeatherHistoryResponce.WindSpeed,
                Country = createWeatherHistoryResponce.Country
            };

            await _weatherHistoryRepository.InsertOneAsync(newWeatherHistory);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
           
        return true;
    }

   
    public async Task<WeatherHistory> GetWeatherHistoryById(Guid id)
    {
        try
        {
            var weatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
            
            return weatherHistory;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task<List<WeatherHistory>> GetReceivedWeatherHistory(long id)
    {
        try
        {
            var receivedWeatherHistory = await _weatherHistoryRepository.FindReceivedWeatherByUserIdAsync(id);
        
            return receivedWeatherHistory?.ToList() ?? new List<WeatherHistory>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Помилка отримання отриманої погоди: {e.Message}");
            throw;
        }
    }


    public async Task<bool> UpdateWeatherHistory(Guid id, CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        try
        {
        
            
            var existingWeatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
            if (existingWeatherHistory == null) return false;

            existingWeatherHistory.UserId = createWeatherHistoryResponce.UserId;
            existingWeatherHistory.City = createWeatherHistoryResponce.City;
            existingWeatherHistory.WeatherDescription = createWeatherHistoryResponce.WeatherDescription;
            existingWeatherHistory.Temperature = createWeatherHistoryResponce.Temperature;
            existingWeatherHistory.FeelsLike = createWeatherHistoryResponce.FeelsLike;
            existingWeatherHistory.Humidity = createWeatherHistoryResponce.Humidity;
            existingWeatherHistory.WindSpeed = createWeatherHistoryResponce.WindSpeed;
            existingWeatherHistory.Country = createWeatherHistoryResponce.Country;

            await _weatherHistoryRepository.UpdateOneAsync(existingWeatherHistory);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    
    }

    public async Task<bool> DeleteWeatherHistory(Guid id)
    {
        try
        {
            var weatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
            if (weatherHistory == null) return false;
            await _weatherHistoryRepository.DeleteOneAsync(id);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }
}