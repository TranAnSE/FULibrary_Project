using BusinessObjects;

namespace DataAccessObjects;

public interface IMagicLinkTokenDAO : IBaseDAO<MagicLinkToken>
{
    Task<MagicLinkToken?> GetByTokenAsync(string token);
    Task<IEnumerable<MagicLinkToken>> GetByUserAsync(Guid userId);
}
