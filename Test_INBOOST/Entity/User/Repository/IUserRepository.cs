using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.User.Repository;

public interface IUserRepository<TBaseData> : IBaseRepository<TBaseData> where TBaseData : BaseData
{
   
    Task DeleteByUserIdAsync(long id);
    Task<User> FindByUserIdAsync(long id);
}