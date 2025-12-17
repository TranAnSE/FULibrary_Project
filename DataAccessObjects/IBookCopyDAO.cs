using BusinessObjects;

namespace DataAccessObjects;

public interface IBookCopyDAO : IBaseDAO<BookCopy>
{
    Task<BookCopy?> GetByRegistrationNumberAsync(string registrationNumber);
    Task<IEnumerable<BookCopy>> GetByBookAsync(Guid bookId);
    Task<IEnumerable<BookCopy>> GetAvailableCopiesAsync(Guid bookId);
    Task<int> GetAvailableCountAsync(Guid bookId);
}
