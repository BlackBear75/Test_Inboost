using System.Data;
using System.Data.SqlClient;
using Dapper;
using Jester.Configuration;
using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.User.Repository;

public class UserRepository<TBaseData> : BaseRepository<TBaseData>, IUserRepository<TBaseData>
    where TBaseData : BaseData
{
    private readonly IDbConnection _db;
    private readonly string _tableName;
    public UserRepository(IConfiguration config) : base(config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        _tableName = typeof(TBaseData).Name+"s" ; 
    }


    public async Task DeleteByUserIdAsync(long userId)
    {
        string sql = $"UPDATE {_tableName} SET Deleted = 1, DeletionDate = @Date WHERE UserId = @UserId";
        await _db.ExecuteAsync(sql, new { UserId = userId, Date = DateTime.UtcNow });
    }

    public async Task<User> FindByUserIdAsync(long userId)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId AND Deleted = 0";
        return await _db.QuerySingleOrDefaultAsync<User>(sql, new { UserId = userId });
    }
}

