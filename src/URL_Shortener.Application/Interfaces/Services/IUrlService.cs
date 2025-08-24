
using URL_Shortener.Application.Dto.Url;

namespace URL_Shortener.Application.Interfaces.Services
{   
    public interface IUrlService
    {
        public Task<string> CreateShortUrlAsync(string longURL);
        public Task<string> GetLongUrlAsync(string shortCode);
        public Task<IEnumerable<UrlListDto>> GetAllAsync();
        public Task<UrlDetailsDto> GetByIdAsync(long id);
        public Task<bool> DeleteByIdAsync(long id);
    }
}
