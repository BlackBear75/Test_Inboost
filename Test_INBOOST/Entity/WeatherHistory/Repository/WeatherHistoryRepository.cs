using System.Data;
using System.Data.SqlClient;
using Dapper;
using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.WeatherHistory.Repository;

public class WeatherHistoryRepository<TBaseData> : BaseRepository<TBaseData>, IWeatherHistoryRepository<TBaseData>
    where TBaseData : BaseData
{
    private readonly IDbConnection _db;
    private readonly string _tableName;

    public WeatherHistoryRepository(IConfiguration config) : base(config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        _tableName = typeof(TBaseData).Name+"s" ; 
    }

    public async Task<IEnumerable<TBaseData>> FindByUserIdAsync(long userId)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId AND Deleted = 0";
        return await _db.QueryAsync<TBaseData>(sql, new { UserId = userId });
    }
    public async Task DeleteByUserIdAsync(Guid id, long userId)
    {
        string sql = $"UPDATE {_tableName} SET Deleted = 1, DeletionDate = @Date WHERE Id = @Id AND UserId = @UserId";
        await _db.ExecuteAsync(sql, new { Id = id, UserId = userId, Date = DateTime.UtcNow });
    }
    public async Task<IEnumerable<TBaseData>> FindReceivedWeatherByUserIdAsync(long userId)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE RecipientUserId = @UserId AND Deleted = 0";
        return await _db.QueryAsync<TBaseData>(sql, new { UserId = userId });
    }

}

