using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;

namespace Test_INBOOST.Service
{
    public interface IWeatherService
    {
        Task<string> GetWeatherAsync(string? city, long userId);
    }

    public class WeatherService : IWeatherService
    {
        private readonly IWeatherHistoryRepository<WeatherHistory> _weatherHistoryRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, string apiKey, IWeatherHistoryRepository<WeatherHistory> weatherHistoryRepository)
        {
            _weatherHistoryRepository = weatherHistoryRepository;
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GetWeatherAsync(string city, long userId)
        {
            var response = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherData = JObject.Parse(content);

            var weatherDescription = weatherData["weather"]?[0]?["description"]?.ToString();
            var temperature = weatherData["main"]?["temp"]?.ToString();
            var feelsLike = weatherData["main"]?["feels_like"]?.ToString();
            var humidity = weatherData["main"]?["humidity"]?.ToString();
            var windSpeed = weatherData["wind"]?["speed"]?.ToString();
            var cityName = weatherData["name"]?.ToString();
            var country = weatherData["sys"]?["country"]?.ToString();

            var formattedWeather = $"Weather in {cityName}, {country}:\n" +
                                   $"- Description: {weatherDescription}\n" +
                                   $"- Temperature: {temperature}°C\n" +
                                   $"- Feels Like: {feelsLike}°C\n" +
                                   $"- Humidity: {humidity}%\n" +
                                   $"- Wind Speed: {windSpeed} m/s";

            await _weatherHistoryRepository.InsertOneAsync(new WeatherHistory()
            {
                City = city,
                UserId = userId,
                WeatherDescription = weatherDescription,
                Temperature = temperature,
                FeelsLike = feelsLike,
                Humidity = humidity,
                WindSpeed = windSpeed,
                Country = country,
            });

            return formattedWeather;
        }
    }
}