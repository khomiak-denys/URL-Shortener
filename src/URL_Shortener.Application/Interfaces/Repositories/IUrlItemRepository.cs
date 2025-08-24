using URL_Shortener.Domain.Entities;

namespace URL_Shortener.Application.Interfaces.Repositories
{
    public interface IUrlItemRepository
    {
        public Task<UrlItem> GetByUrlAsync(string shortUrl);
        public Task<UrlItem> AddAsync(UrlItem item);
        public Task<IEnumerable<UrlItem>> GetAllAsync();
        public Task<UrlItem> GetByIdAsync(long id);
        public Task<bool> DeleteById(long id);
    }
}
