using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;
using Test_INBOOST.Models.UsersModel;
using Test_INBOOST.Models.WeatherHistoryModel;

namespace Test_INBOOST.Service;

public interface IWeatherHistoryService
{
    Task<List<WeatherHistory>> GetAllUserWeatherHistory(long userId);
    Task<bool> CreateWeatherHistory(CreateWeatherHistoryResponce createWeatherHistoryResponce);
    Task<bool> DeleteWeatherHistory(Guid id, long userId);

    Task<WeatherHistory> GetWeatherHistoryById(Guid id);

    Task<bool> UpdateWeatherHistory(Guid id,
        [FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce);

    Task<bool> DeleteWeatherHistory(Guid id);
}

public class WeatherHistoryService : IWeatherHistoryService
{
    private readonly IWeatherHistoryRepository<WeatherHistory> _weatherHistoryRepository;


    public WeatherHistoryService(IWeatherHistoryRepository<WeatherHistory> weatherHistoryRepository)
    {
        _weatherHistoryRepository = weatherHistoryRepository;
     
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
            WeatherHistory newWeatherHistory = new WeatherHistory()
            {
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

    public async Task<bool> DeleteWeatherHistory(Guid id, long userId)
    {
        try
        {
            var weatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
            if (weatherHistory == null) return false;
        
            await _weatherHistoryRepository.DeleteByUserIdAsync(id, userId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
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

    public async Task<bool> UpdateWeatherHistory(Guid id, CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        try
        {
            var existingWeatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
            if (existingWeatherHistory == null) return false;

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