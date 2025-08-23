using URL_Shortener.Domain.Entities;
using URL_Shortener.Application.Interfaces.Repositories;
using URL_Shortener.Infrastructure.Database;

namespace URL_Shortener.Infrastructure.Repositories
{
    public class EFUrlItemRepository : IUrlItemRepository
    {
        private readonly AppDbContext _dbContext;
        public EFUrlItemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UrlItem> GetByUrlAsync(string shortUrl)
        {
            var item = _dbContext.UrlItems.Where(i => i.ShortUrl.Contains(shortUrl)).FirstOrDefault();
            return item;
        }
        public async Task<UrlItem> AddAsync(UrlItem item)
        {
            await _dbContext.UrlItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }
    }
}
