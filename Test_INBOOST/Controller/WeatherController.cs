using Microsoft.AspNetCore.Mvc;
using Test_INBOOST.Service;

namespace Test_INBOOST.Controller;


[ApiController]
[Route("api/users")]
public class WeatherController : ControllerBase
{
    IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    
    [HttpGet("GetWeather")]
    public async Task<IActionResult> GetWeather(string city,long userrid)
    {
     var res = await _weatherService.GetWeatherAsync(city,userrid);
      return Ok(res);
    }

    [HttpGet("SendWeather")]
    public async Task<IActionResult> SendWeather(string city, long userId, long recepientId)
    {
        
        return Ok(await _weatherService.SendWeather(city,userId,recepientId));
    }
    
}