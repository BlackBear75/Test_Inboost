using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.WeatherHistory.Repository;

public interface IWeatherHistoryRepository<TBaseData> : IBaseRepository<TBaseData> where TBaseData : BaseData
{
   
    Task DeleteByUserIdAsync(Guid id,long userId);
}