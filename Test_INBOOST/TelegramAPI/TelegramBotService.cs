using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Models.UsersModel;
using Test_INBOOST.Service;
using User = Test_INBOOST.Entity.User.User;

public class TelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IWeatherService _weatherService;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserService _userService;

    public TelegramBotService(ITelegramBotClient botClient, IWeatherService weatherService, IUserRepository<User> userRepository, IUserService userService)
    {
        _userService = userService;
        _userRepository = userRepository;
        _botClient = botClient;
        _weatherService = weatherService;
    }

    public void StartPolling()
    {
        _botClient.StartReceiving(
            updateHandler: HandleUpdatesAsync,    
            errorHandler: HandleErrorAsync,      
            cancellationToken: CancellationToken.None
        );
    }

   private async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update?.Message?.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text.Trim();

            if (messageText.StartsWith("/start"))
            {
                var existingUser = await _userRepository.FindByUserIdAsync(chatId);
               
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        UserId = update.Message.From.Id,
                        UserName = update.Message.From.Username,
                        FirstName = update.Message.Chat.FirstName,
                        LastName = update.Message.Chat.LastName
                    };

                    await _userRepository.InsertOneAsync(newUser);
                }
                
                await botClient.SendTextMessageAsync(chatId, "Доброго дня!");
            }
            else if (messageText.StartsWith("/weather"))
            {
                var city = messageText.Replace("/weather", "").Trim();
                if (string.IsNullOrEmpty(city))
                {
                    await botClient.SendTextMessageAsync(chatId, "Будь ласка уведіть місто");
                    return;
                }

                var weatherResponse = await _weatherService.GetWeatherAsync(city, chatId);
                await botClient.SendTextMessageAsync(chatId, weatherResponse);
            }
            else if (messageText.StartsWith("/userandweather"))
            {
                var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2 || !long.TryParse(parts[1], out long userId))
                {
                    await botClient.SendTextMessageAsync(chatId, "Невірний формат. Використовуйте /userandweather {userid}");
                    return;
                }

                var userAndWeatherHistory = await _userService.GetUserAndWeatherHistory(Guid.Empty, userId);

                if (userAndWeatherHistory == null || !userAndWeatherHistory.Any())
                {
                    await botClient.SendTextMessageAsync(chatId, "Інформація не знайдена.");
                    return;
                }

                var responseText = new StringBuilder();

                foreach (var item in userAndWeatherHistory)
                {
                    responseText.AppendLine($"👤 *Користувач:* {(item.FirstName)} {(item.LastName)} (@{(item.UserName)})");
                    responseText.AppendLine("🌦 *Історія погоди:*");

                    foreach (var history in item.WeatherHistory)
                    {
                        responseText.AppendLine($"📅 *{history.Date:yyyy-MM-dd}*  {(history.City)} ({(history.Country)})");
                        responseText.AppendLine($"   ☀ {(history.WeatherDescription)}");
                        responseText.AppendLine($"   🌡 Температура: {(history.Temperature)}°C (Відчувається як {(history.FeelsLike)}°C)");
                        responseText.AppendLine($"   💧 Вологість: {(history.Humidity)}%");
                        responseText.AppendLine($"   💨 Вітер: {(history.WindSpeed)} м/с");
                        responseText.AppendLine();
                    }

                    responseText.AppendLine(new string('-', 30)); // Додаємо роздільник
                }

                await botClient.SendTextMessageAsync(chatId, EscapeMarkdownV2(responseText.ToString()), parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }



            else
            {
                var responseText = "Некоректне повідомлення. Напишіть /weather {city} або /userandweather {userid}.";
                await botClient.SendTextMessageAsync(chatId, responseText);
            }
        }
    }

    private string EscapeMarkdownV2(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        // Список символів для екранування в MarkdownV2
        var specialCharacters = new HashSet<char> {
            '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'
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







    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}