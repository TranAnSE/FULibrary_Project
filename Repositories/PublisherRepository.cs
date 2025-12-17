using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class PublisherRepository : Repository<Publisher>, IPublisherRepository
{
    private readonly IPublisherDAO _publisherDAO;

    public PublisherRepository(IPublisherDAO publisherDAO) : base(publisherDAO)
    {
        _publisherDAO = publisherDAO;
    }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _publisherDAO.GetByNameAsync(name);
    }
}
