using Test_INBOOST.Base;
using Test_INBOOST.Base.Repository;

namespace Test_INBOOST.Entity.WeatherHistory.Repository;

public interface IWeatherHistoryRepository<TBaseData> : IBaseRepository<TBaseData> where TBaseData : BaseData
{
    Task<IEnumerable<TBaseData>> FindByUserIdAsync(long userId);
    Task DeleteByUserIdAsync(Guid id,long userId);
   
    Task<IEnumerable<TBaseData>> FindReceivedWeatherByUserIdAsync(long userId);
    

}