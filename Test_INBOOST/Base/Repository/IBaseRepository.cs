namespace Test_INBOOST.Base.Repository;

public interface IBaseRepository<TBaseData> where TBaseData : BaseData
{
    Task<IEnumerable<TBaseData>> GetAllAsync();
    Task<TBaseData> FindByIdAsync(long id);
    Task InsertOneAsync(TBaseData document);
    Task UpdateOneAsync(TBaseData document);
    Task DeleteOneAsync(long id);
    Task<string> GetConnectionString();

}