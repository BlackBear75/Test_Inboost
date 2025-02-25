using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Dapper;
using Jester.Configuration;

namespace Test_INBOOST.Base.Repository;

public class BaseRepository<TBaseData> : IBaseRepository<TBaseData> where TBaseData : BaseData
{
    private readonly IDbConnection _db;
    private readonly string _tableName;

    public BaseRepository(IConfiguration config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        _tableName = typeof(TBaseData).Name + "s"; 
    }

    public async Task<string> GetConnectionString()
    {
        return _db.ConnectionString;
    }

    public async Task<IEnumerable<TBaseData>> GetAllAsync()
    {
        string sql = $"SELECT * FROM {_tableName} WHERE Deleted = 0";
        return await _db.QueryAsync<TBaseData>(sql);
    }

    public async Task<TBaseData> FindByIdAsync(long id)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id AND Deleted = 0";
        return await _db.QuerySingleOrDefaultAsync<TBaseData>(sql, new { Id = id });
    }

    public async Task InsertOneAsync(TBaseData document)
    {
        document.Deleted = false;

        string sql = $"INSERT INTO {_tableName} ({GetColumnNames()}) VALUES ({GetColumnParams()})";
        await _db.ExecuteAsync(sql, document);
    }

    public async Task UpdateOneAsync(TBaseData document)
    {
        string sql = $"UPDATE {_tableName} SET {GetUpdateColumns()} WHERE Id = @Id";
        await _db.ExecuteAsync(sql, document);
    }

    public async Task DeleteOneAsync(long id)
    {
        string sql = $"UPDATE {_tableName} SET Deleted = 1, DeletionDate = @Date WHERE Id = @Id";
        await _db.ExecuteAsync(sql, new { Id = id, Date = DateTime.UtcNow });
    }

    private string GetColumnNames()
    {
        return string.Join(", ", typeof(TBaseData).GetProperties()
            .Where(p =>   p.Name != "DeletionDate")
            .Select(p => p.Name));
    }

    private string GetColumnParams()
    {
        return string.Join(", ", typeof(TBaseData).GetProperties()
            .Where(p =>  p.Name != "DeletionDate")
            .Select(p => "@" + p.Name));
    }

    private string GetUpdateColumns()
    {
        return string.Join(", ", typeof(TBaseData).GetProperties()
            .Where(p =>   p.Name != "DeletionDate")
            .Select(p => $"{p.Name} = @{p.Name}"));
    }
}
