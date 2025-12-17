using BusinessObjects;

namespace Repositories;

public interface IPublisherRepository : IRepository<Publisher>
{
    Task<Publisher?> GetByNameAsync(string name);
}
