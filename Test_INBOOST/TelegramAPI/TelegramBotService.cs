using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Test_INBOOST.Entity.User;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Helper;
using Test_INBOOST.Service;
using User = Test_INBOOST.Entity.User.User;

namespace Test_INBOOST.TelegramAPI;

public class TelegramBotService
{
    private Dictionary<long, long> _pendingWeatherRequests = new Dictionary<long, long>();
    private readonly ITelegramBotClient _botClient;
    private readonly IWeatherService _weatherService;
    private readonly IWeatherHistoryService _weatherHistoryService;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserService _userService;

    private readonly ReplyKeyboardMarkup _mainKeyboard = new(new[]
    {
        new KeyboardButton[] { "🌦 Погода", "📤 Відправити погоду" },
        new KeyboardButton[] { "📨 Отримана погода", "📊 Історія погоди" },
        new KeyboardButton[] { "👥 Користувачі", "ℹ️ Допомога" }
    })
    {
        ResizeKeyboard = true
    };

    public TelegramBotService(ITelegramBotClient botClient, IWeatherService weatherService, 
        IUserRepository<User> userRepository, IUserService userService, 
        IWeatherHistoryService weatherHistoryService)
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
        if (update.Message is { } message)
        {
            var chatId = message.Chat.Id;
            
            await EnsureUserExists(message);
            
            if (message.Text == "/start")
            {
                await HandleStartCommand(chatId);
                return;
            }

            if (message.ReplyToMessage?.Text?.Contains("Введіть назву міста:") ?? false)
            {
                await HandleCityInput(chatId, message.Text);
                return;
            }

            switch (message.Text)
            {
                case "🌦 Погода":
                    await RequestCityInput(chatId, "weather");
                    break;
                case "📤 Відправити погоду":
                    await RequestCityAndUserInput(chatId);
                    break;
                case "📨 Отримана погода":
                    await ShowReceivedWeather(chatId);
                    break;
                case "📊 Історія погоди":
                     await RequestUserForHistory(chatId);
                    break;
                case "👥 Користувачі":
                    await ShowUsers(chatId);
                    break;
                case "ℹ️ Допомога":
                    await ShowHelp(chatId);
                    break;
                default:
                    await botClient.SendTextMessageAsync(chatId, "Оберіть дію з меню 👇", 
                        replyMarkup: _mainKeyboard);
                    break;
            }
        }
        else if (update.CallbackQuery is { } callbackQuery)
        {
            await HandleCallbackQuery(callbackQuery);
        }
    }
    private async Task EnsureUserExists(Message message)
    {
        var chatId = message.Chat.Id;
        var existingUser  = await _userRepository.FindByUserIdAsync(chatId);
        if (existingUser  == null)
        {
            var newUser  = new User
            {
                UserId = message.From.Id,
                UserName = message.From.Username,
                FirstName = message.Chat.FirstName,
                LastName = message.Chat.LastName,
                Role = UserRole.User
            };
            await _userRepository.InsertOneAsync(newUser);
        }
    }
    private async Task HandleStartCommand(long chatId)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            "👋 Доброго дня! Оберіть дію з меню 👇",
            replyMarkup: _mainKeyboard);
    }

    private async Task RequestCityInput(long chatId, string action)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            "Введіть назву міста:",
            replyMarkup: new ForceReplyMarkup { Selective = true });
    }

    private async Task HandleCityInput(long chatId, string city)
    {
        if (_pendingWeatherRequests.TryGetValue(chatId, out long recipientId))
        {
            var result = await _weatherService.SendWeather(city, chatId, recipientId);

            await _botClient.SendTextMessageAsync(chatId, result, replyMarkup: _mainKeyboard);

            _pendingWeatherRequests.Remove(chatId);
        }
        else
        {
            var weatherResponse = await _weatherService.GetWeatherAsync(city, chatId);

            var weatherText = new StringBuilder();
            
            var user = await _userRepository.FindByUserIdAsync(chatId);
            weatherText.AppendLine(HelperFormating.FormatWeatherMessage(weatherResponse,user));

            await _botClient.SendTextMessageAsync(
                chatId, 
                HelperFormating.EscapeMarkdownV2(weatherText.ToString()), 
                parseMode: ParseMode.MarkdownV2, 
                replyMarkup: _mainKeyboard);
        }
    }

    private async Task RequestCityAndUserInput(long chatId)
    {
        var users = await _userService.GetAllUsers();
        var buttons = users.Select(u => 
                InlineKeyboardButton.WithCallbackData($"{u.FirstName} {u.LastName}", $"send_to_{u.UserId}"))
            .Chunk(2)
            .ToList();

        await _botClient.SendTextMessageAsync(
            chatId,
            "Оберіть користувача:",
            replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
    {
        var data = callbackQuery.Data;
        var chatId = callbackQuery.Message.Chat.Id;

        if (data.StartsWith("send_to_"))
        {
            var recipientId = long.Parse(data.Split('_')[2]);
            await _botClient.SendTextMessageAsync(
                chatId,
                "Введіть назву міста:",
                replyMarkup: new ForceReplyMarkup { Selective = true });

            _pendingWeatherRequests[chatId] = recipientId;
        }
    }
    private async Task RequestUserForHistory(long chatId)
    {
        var history = await _userService.GetUserAndWeatherHistory(Guid.Empty, chatId);

        if (history == null || !history.Any())
        {
            await _botClient.SendTextMessageAsync(chatId, "📭 Історія погоди не знайдена.");
            return;
        }

        var responseText = new StringBuilder("📊 Ваша історія погоди:\n\n");
        foreach (var userHistory in history)
        {
            responseText.AppendLine($"👤 {userHistory.FirstName} {userHistory.LastName} (@{userHistory.UserName})");
            foreach (var weather in userHistory.WeatherHistory)
            {
                
                responseText.AppendLine(HelperFormating.FormatWeatherMessage(weather));
            }
        }

        await _botClient.SendTextMessageAsync(chatId, HelperFormating.EscapeMarkdownV2(responseText.ToString()), parseMode: ParseMode.MarkdownV2);
    }
    private async Task ShowReceivedWeather(long chatId)
    {
        var receivedWeatherHistory = await _weatherHistoryService.GetReceivedWeatherHistory(chatId);

        if (!receivedWeatherHistory.Any())
        {
            await _botClient.SendTextMessageAsync(chatId, "📭 Ви ще не отримували погоду.");
            return;
        }

        var response = new StringBuilder("📨 Отримана погода:\n\n");
        foreach (var weather in receivedWeatherHistory)
        {
            var user = await _userRepository.FindByUserIdAsync(weather.UserId);
             
            response.Append(HelperFormating.FormatWeatherMessage(weather,user));
        }

        await _botClient.SendTextMessageAsync(chatId, response.ToString(), replyMarkup: _mainKeyboard);
    }

    private async Task ShowHelp(long chatId)
    {
        var helpText = new StringBuilder("📖 Доступні команди:\n\n");
        helpText.AppendLine("🌦 Погода - Отримати погоду");
        helpText.AppendLine("📤 Відправити погоду - Поділитись погодою");
        helpText.AppendLine("📨 Отримана погода - Переглянути історію");
        helpText.AppendLine("📊 Історія погоди - Історія запитів");
        helpText.AppendLine("👥 Користувачі - Список користувачів");

        await _botClient.SendTextMessageAsync(chatId, helpText.ToString(), replyMarkup: _mainKeyboard);
    }

    private async Task ShowUsers(long chatId)
    {
        var users = await _userService.GetAllUsers();

        if (users == null || !users.Any())
        {
            await _botClient.SendTextMessageAsync(chatId, "📭 Список користувачів порожній.", replyMarkup: _mainKeyboard);
            return;
        }

        var responseText = new StringBuilder("📋 Список користувачів:\n\n");
        int number = 1;
        foreach (var user in users)
        {
            responseText.AppendLine($"{number}👤 {user.FirstName} {user.LastName} (@{user.UserName})\n Роль : {user.Role}\n Id : {user.UserId}");
            number++;
        }

        await _botClient.SendTextMessageAsync(chatId,HelperFormating.EscapeMarkdownV2(responseText.ToString()), parseMode: ParseMode.MarkdownV2, replyMarkup: _mainKeyboard);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}
