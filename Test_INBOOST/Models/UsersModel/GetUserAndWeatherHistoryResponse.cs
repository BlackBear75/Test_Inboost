using Test_INBOOST.Entity.WeatherHistory;
using Test_INBOOST.Models.WeatherHistoryModel;

namespace Test_INBOOST.Models.UsersModel;

public class GetUserAndWeatherHistoryResponse
{
    public long UserId { get; set; }
    
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<GetWeatherHistoryResponce> WeatherHistory { get; set; }
}

    
