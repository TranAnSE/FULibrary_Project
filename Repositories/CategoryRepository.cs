using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ICategoryDAO _categoryDAO;

    public CategoryRepository(ICategoryDAO categoryDAO) : base(categoryDAO)
    {
        _categoryDAO = categoryDAO;
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _categoryDAO.GetByNameAsync(name);
    }
}
