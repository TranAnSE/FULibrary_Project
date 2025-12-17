using BusinessObjects;

namespace DataAccessObjects;

public interface ILanguageDAO : IBaseDAO<Language>
{
    Task<Language?> GetByNameAsync(string name);
    Task<Language?> GetByCodeAsync(string code);
}
