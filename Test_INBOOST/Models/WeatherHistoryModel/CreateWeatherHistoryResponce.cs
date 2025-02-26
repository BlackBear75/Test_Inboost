namespace Test_INBOOST.Models.WeatherHistoryModel;

public class CreateWeatherHistoryResponce
{
    public string City { get; set; } 
    public string WeatherDescription { get; set; }
    public string Temperature { get; set; }
    public string FeelsLike { get; set; }
    public string Humidity { get; set; }
    public string WindSpeed { get; set; }
    public string Country { get; set; }
}