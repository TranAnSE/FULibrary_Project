using BusinessObjects;

namespace Repositories;

public interface ILanguageRepository : IRepository<Language>
{
    Task<Language?> GetByNameAsync(string name);
    Task<Language?> GetByCodeAsync(string code);
}
