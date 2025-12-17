using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class LanguageRepository : Repository<Language>, ILanguageRepository
{
    private readonly ILanguageDAO _languageDAO;

    public LanguageRepository(ILanguageDAO languageDAO) : base(languageDAO)
    {
        _languageDAO = languageDAO;
    }

    public async Task<Language?> GetByNameAsync(string name)
    {
        return await _languageDAO.GetByNameAsync(name);
    }

    public async Task<Language?> GetByCodeAsync(string code)
    {
        return await _languageDAO.GetByCodeAsync(code);
    }
}
