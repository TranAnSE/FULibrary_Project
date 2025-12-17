using BusinessObjects;

namespace DataAccessObjects;

public interface IOtpTokenDAO : IBaseDAO<OtpToken>
{
    Task<OtpToken?> GetByEmailAndCodeAsync(string email, string code);
    Task<IEnumerable<OtpToken>> GetByEmailAsync(string email);
}
