using System.Data;
using System.Data.SqlClient;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Entity.WeatherHistory.Repository;
using Test_INBOOST.Service;

namespace Test_INBOOST.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        ConfigureService(builder.Services);
        WeatherConfiguration(builder.Services, builder.Configuration);
        
    }
     private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
     {
         services.AddScoped<IDbConnection>(sp =>
             new SqlConnection(configuration.GetConnectionString("DefaultConnection")));


         var dbInitializer = new DatabaseInitializer(configuration.GetConnectionString("DefaultConnection"));
         dbInitializer.Initialize();
     }
    
    private static void ConfigureService(IServiceCollection services)
    {
        
        services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
        
        services.AddScoped(typeof(IWeatherHistoryRepository<>), typeof(WeatherHistoryRepository<>));
        
        
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IWeatherHistoryService, WeatherHistoryService>();
        
        
    }

    private static void WeatherConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
            });

        services.AddTransient<IWeatherService, WeatherService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var apiKey = configuration["WeatherApiKey"];
            var weatherHistoryRepository = provider.GetRequiredService<IWeatherHistoryRepository<WeatherHistory>>();
            return new WeatherService(httpClient, apiKey, weatherHistoryRepository);
        });
    }
    
    
 
}