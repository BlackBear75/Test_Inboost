namespace Test_INBOOST.Base.Repository;

public interface IBaseRepository<TBaseData> where TBaseData : BaseData
{
    Task<IEnumerable<TBaseData>> GetAllAsync();
    Task<TBaseData> FindByIdAsync(Guid id);
    Task InsertOneAsync(TBaseData document);
    Task UpdateOneAsync(TBaseData document);
    Task DeleteOneAsync(Guid id);
    Task<string> GetConnectionString();

}