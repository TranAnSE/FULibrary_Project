using BusinessObjects;

namespace DataAccessObjects;

public interface ICategoryDAO : IBaseDAO<Category>
{
    Task<Category?> GetByNameAsync(string name);
}
