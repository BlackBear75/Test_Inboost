using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Service;
using User = Test_INBOOST.Entity.User.User;

namespace Test_INBOOST.TelegramAPI;

public class TelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IWeatherService _weatherService;
    
    private readonly IWeatherHistoryService _weatherHistoryService;
    
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserService _userService;

    public TelegramBotService(ITelegramBotClient botClient, IWeatherService weatherService, IUserRepository<User> userRepository, IUserService userService, IWeatherHistoryService weatherHistoryService)
    {
        _userService = userService;
        _userRepository = userRepository;
        _botClient = botClient;
        _weatherService = weatherService;
        _weatherHistoryService = weatherHistoryService;
        
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

            await botClient.SendTextMessageAsync(chatId, "👋 Доброго дня! Напишіть /help, щоб переглянути команди.");
        }
        else if (messageText.StartsWith("/weather"))
        {
            var city = messageText.Replace("/weather", "").Trim();
            if (string.IsNullOrEmpty(city))
            {
                await botClient.SendTextMessageAsync(chatId, "❌ Будь ласка, введіть місто.");
                return;
            }

            var weatherResponse = await _weatherService.GetWeatherAsync(city, chatId);
            await botClient.SendTextMessageAsync(chatId, weatherResponse);
        }
        else if (messageText.StartsWith("/sendweather"))
        {
            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3 || !long.TryParse(parts[1], out long recipientId))
            {
                await botClient.SendTextMessageAsync(chatId, "❌ Невірний формат. Використовуйте: /sendweather {userId} {місто}");
                return;
            }

            var city = string.Join(" ", parts.Skip(2)); 
            await _weatherService.SendWeather(city, chatId, recipientId);

            await botClient.SendTextMessageAsync(chatId, $"✅ Погода для {city} успішно відправлена користувачу з id {recipientId}.");
        }
        else if (messageText.StartsWith("/receivedweather"))
        {
            var receivedWeatherHistory = await _weatherHistoryService.GetReceivedWeatherHistory(chatId);

            if (receivedWeatherHistory == null || !receivedWeatherHistory.Any())
            {
                await botClient.SendTextMessageAsync(chatId, "📭 Ви ще не отримували погоду від інших користувачів.");
                return;
            }

            var responseText = new StringBuilder("📨 *Отримана погода:*\n\n");
          
            foreach (var weather in receivedWeatherHistory)
            {
                var recepientuser = await _userRepository.FindByUserIdAsync(chatId);
                responseText.AppendLine($"📅 *{weather.CreationDate:yyyy-MM-dd}* від користувача : {recepientuser.FirstName} {recepientuser.LastName}");
                responseText.AppendLine($"🌍 {weather.City}, {weather.Country}");
                responseText.AppendLine($"☁ {weather.WeatherDescription}");
                responseText.AppendLine($"🌡 Температура: {weather.Temperature}°C (відчувається як {weather.FeelsLike}°C)");
                responseText.AppendLine($"💧 Вологість: {weather.Humidity}%");
                responseText.AppendLine($"💨 Вітер: {weather.WindSpeed} м/с");
                responseText.AppendLine(new string('-', 30));
            }

            await botClient.SendTextMessageAsync(chatId, EscapeMarkdownV2(responseText.ToString()), parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        else if (messageText.StartsWith("/userandweather"))
        {
            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !long.TryParse(parts[1], out long userId))
            {
                await botClient.SendTextMessageAsync(chatId, "❌ Невірний формат. Використовуйте: /userandweather {userId}");
                return;
            }

            var userAndWeatherHistory = await _userService.GetUserAndWeatherHistory(Guid.Empty, userId);

            if (userAndWeatherHistory == null || !userAndWeatherHistory.Any())
            {
                await botClient.SendTextMessageAsync(chatId, "📭 Інформація не знайдена.");
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

                responseText.AppendLine(new string('-', 30)); 
            }

            await botClient.SendTextMessageAsync(chatId, EscapeMarkdownV2(responseText.ToString()), parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        else if (messageText.StartsWith("/users"))
        {
            var users = await _userService.GetAllUsers();

            if (users == null || !users.Any())
            {
                await botClient.SendTextMessageAsync(chatId, "📭 Список користувачів порожній.");
                return;
            }

            var responseText = new StringBuilder("📋 *Список користувачів:*\n\n");

            int number = 1;
            foreach (var user in users)
            {
                responseText.AppendLine($"{number}👤 {user.FirstName} {user.LastName} (@{user.UserName})\n Роль : {user.Role}\n Id : {user.UserId}");
                number++;
            }

            await botClient.SendTextMessageAsync(chatId, EscapeMarkdownV2(responseText.ToString()), parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        else if (messageText.StartsWith("/help"))
        {
            var helpText = new StringBuilder("📖 *Доступні команди:*\n\n");
            helpText.AppendLine("/start - Запуск бота");
            helpText.AppendLine("/weather {місто} - Отримати погоду у вказаному місті");
            helpText.AppendLine("/sendweather {userId} {місто} - Відправити погоду іншому користувачу");
            helpText.AppendLine("/receivedweather - Переглянути отриману погоду");
            helpText.AppendLine("/userandweather {userId} - Отримати історію погоди користувача");
            helpText.AppendLine("/users - Отримати список користувачів");
            helpText.AppendLine("/help - Показати список доступних команд");

            await botClient.SendTextMessageAsync(chatId, EscapeMarkdownV2(helpText.ToString()), parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, "❌ Невідома команда. Напишіть /help для списку доступних команд.");
        }
    }
}

    private string EscapeMarkdownV2(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

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