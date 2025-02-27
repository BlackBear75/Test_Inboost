using System.Text;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.WeatherHistory;

namespace Test_INBOOST.Helper;

public static class HelperFormating
{
    public static string FormatWeatherMessage(WeatherHistory weather,User user=null)
    {
        var weatherText = new StringBuilder();


        if (user!=null)
        {
            weatherText.AppendLine($"👤  {user.FirstName} {user.LastName} @{user.UserName} ");
        }
        
        
        weatherText.AppendLine($"📅 {weather.CreationDate:yyyy-MM-dd}");
        weatherText.AppendLine($"🌆 *Місто:* {weather.City}, {weather.Country}");
        weatherText.AppendLine($"🌡 *Температура:* {weather.Temperature}°C (Відчувається як {weather.FeelsLike}°C)");
        weatherText.AppendLine($"☁️ *Опис:* {weather.WeatherDescription}");
        weatherText.AppendLine($"💧 *Вологість:* {weather.Humidity}%");
        weatherText.AppendLine($"💨 *Вітер:* {weather.WindSpeed} м/с");
        
        weatherText.AppendLine(new string('-', 20));
        
        return weatherText.ToString();
    }
    public static string EscapeMarkdownV2(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        var specialCharacters = new HashSet<char> {
            '_', '*', '[', ']', '(', ')', '~', ' ', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'
        };

        var escapedText = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            if (specialCharacters.Contains(c))
            {
                escapedText.Append('\\');
            }
            escapedText.Append(c);
        }

        return escapedText.ToString();
    }
}