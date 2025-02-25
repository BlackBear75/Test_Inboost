using System.Data;
using System.Data.SqlClient;
using Jester.Configuration;
using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.User.Repository;

public class UserRepository<TBaseData> : BaseRepository<TBaseData>, IUserRepository<TBaseData>
    where TBaseData : BaseData
{
    private readonly IDbConnection _db;

    public UserRepository(IConfiguration config) : base(config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }
    
    
}

