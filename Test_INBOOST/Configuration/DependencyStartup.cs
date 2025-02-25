using System.Data;
using System.Data.SqlClient;
using Test_INBOOST.Entity.User.Repository;
using Test_INBOOST.Entity.WeatherHistory.Repository;

namespace Test_INBOOST.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        ConfigureService(builder.Services);
        
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
        
    }
    
    
    
 
}