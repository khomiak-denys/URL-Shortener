
namespace URL_Shortener.Application.Interfaces.Services
{   
    public interface IUrlService
    {
        public Task<string> CreateShortUrl(string longURL);
        public Task<string> GetLongUrl(string shortCode);
    }
}
