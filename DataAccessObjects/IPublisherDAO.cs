using BusinessObjects;

namespace DataAccessObjects;

public interface IPublisherDAO : IBaseDAO<Publisher>
{
    Task<Publisher?> GetByNameAsync(string name);
}
