using Microsoft.Extensions.Configuration;

namespace Jester.Configuration;

public class EnvironmentSettings
{
    public string EnvironmentName { get; private set; }
    public string ConnectionString { get; private set; }
    public string TelegramBotToken { get; private set; }

    public EnvironmentSettings(IConfiguration configuration)
    {
        EnvironmentName = configuration["EnvironmentName"] ?? "Development";
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
        TelegramBotToken = configuration["TelegramBotToken"];
    }

    public bool IsDevelopment => EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
    public bool IsProduction => EnvironmentName.Equals("Production", StringComparison.OrdinalIgnoreCase);

}