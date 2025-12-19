using BusinessObjects;

namespace DataAccessObjects;

public interface IRoleDAO : IBaseDAO<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
