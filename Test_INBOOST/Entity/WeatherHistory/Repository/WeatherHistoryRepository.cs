using System.Data;
using System.Data.SqlClient;
using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.WeatherHistory.Repository;

public class WeatherHistoryRepository<TBaseData> : BaseRepository<TBaseData>, IWeatherHistoryRepository<TBaseData>
    where TBaseData : BaseData
{
    private readonly IDbConnection _db;

    public WeatherHistoryRepository(IConfiguration config) : base(config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }
    
    
}

