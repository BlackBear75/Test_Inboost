using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;
using Test_INBOOST.Models.WeatherHistoryModel;
using Test_INBOOST.Service;


namespace Test_INBOOST.Controller;

[ApiController]
[Route("api/weather")]
public class WeatherHistoryController : ControllerBase
{
    private readonly IWeatherHistoryService _weatherService;

    public WeatherHistoryController(IWeatherHistoryService weatherService)
    {
         _weatherService = weatherService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateWeatherHistory([FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        if (await _weatherService.CreateWeatherHistory(createWeatherHistoryResponce))
        {
            return Ok("Запис погоди додано.");
        }
        
        return BadRequest("Користувача з таким айд не існує");
    }

    [HttpGet("GetUserWeatherHistory/{userId}")]
    public async Task<IActionResult> GetAllWeatherHistory(long userId)
    {
        var result = await _weatherService.GetAllUserWeatherHistory(userId);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWeatherHistoryById(Guid id)
    {
        return Ok( await _weatherService.GetWeatherHistoryById(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWeatherHistory(Guid id, [FromBody] CreateWeatherHistoryResponce createWeatherHistoryResponce)
    {
        if (await _weatherService.UpdateWeatherHistory(id, createWeatherHistoryResponce))
        {
            
            return Ok("Запис погоди оновлено.");
        }

        return BadRequest("Помилка оновлення немає такого запису");
    }

    [HttpDelete("DeleteById/{id}")]
    public async Task<IActionResult> DeleteWeatherHistory(Guid id)
    {
        if (await _weatherService.DeleteWeatherHistory(id))
        {
            return Ok("Запис погоди видалено.");
        }

        return NotFound("Не має такого обєкта");

    }
    
    [HttpGet("GetReceivedWeatherHistory/{id}")]
    public async Task<IActionResult> GetReceivedWeatherHistory(long id)
    {

        return Ok(await _weatherService.GetReceivedWeatherHistory(id));

    }
}