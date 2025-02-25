using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;
using Test_INBOOST.Models.WeatherHistoryModel;


namespace Test_INBOOST.Controller;

[ApiController]
[Route("api/weather")]
public class WeatherHistoryController : ControllerBase
{
    private readonly IWeatherHistoryRepository<WeatherHistory> _weatherHistoryRepository;

    public WeatherHistoryController(IWeatherHistoryRepository<WeatherHistory> weatherHistoryRepository)
    {
        _weatherHistoryRepository = weatherHistoryRepository;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateWeatherHistory([FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        WeatherHistory newWeatherHistory = new WeatherHistory()
        {
            City = createWeatherHistoryResponce.City,
            WeatherData = createWeatherHistoryResponce.WeatherData,
        };

        await _weatherHistoryRepository.InsertOneAsync(newWeatherHistory);
        return Ok("Запис погоди додано.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWeatherHistory()
    {
        var weatherHistoryList = await _weatherHistoryRepository.GetAllAsync();
        var result = new List<GetWeatherHistoryResponce>();

        foreach (var weatherHistory in weatherHistoryList)
        {
            result.Add(new GetWeatherHistoryResponce()
            {
              Id = weatherHistory.Id,
              City = weatherHistory.City,
              WeatherData = weatherHistory.WeatherData,
            });
        }
        return Ok(weatherHistoryList);
    }
    
    [HttpDelete("DeleteByUserId/{id}/{userId}")]
    public async Task<IActionResult> DeleteWeatherHistory(Guid id,long userId)
    {
        var user = await _weatherHistoryRepository.FindByIdAsync(id);
        if (user == null) return NotFound("Запис погоди не знайдений.");
        
        
        await _weatherHistoryRepository.DeleteByUserIdAsync(id, userId);
        return Ok("Запис погоди видалений.");
    }
    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWeatherHistoryById(Guid id)
    {
        var weatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
        if (weatherHistory == null) return NotFound("Запис погоди не знайдений.");

        return Ok(weatherHistory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWeatherHistory(Guid id, [FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        var existingWeatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
        if (existingWeatherHistory == null) return NotFound("Запис погоди не знайдений.");

        existingWeatherHistory.City = createWeatherHistoryResponce.City;
        existingWeatherHistory.WeatherData = createWeatherHistoryResponce.WeatherData;

        await _weatherHistoryRepository.UpdateOneAsync(existingWeatherHistory);
        return Ok("Запис погоди оновлено.");
    }

    [HttpDelete("DeleteById/{id}")]
    public async Task<IActionResult> DeleteWeatherHistory(Guid id)
    {
        var weatherHistory = await _weatherHistoryRepository.FindByIdAsync(id);
        if (weatherHistory == null) return NotFound("Запис погоди не знайдений.");

        await _weatherHistoryRepository.DeleteOneAsync(id);
        return Ok("Запис погоди видалено.");
    }
}