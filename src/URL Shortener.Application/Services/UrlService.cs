using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using URL_Shortener.Application.Exceptions;
using URL_Shortener.Application.Interfaces.Repositories;
using URL_Shortener.Application.Interfaces.Services;
using URL_Shortener.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace URL_Shortener.Application.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlItemRepository _urlRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public UrlService(
            IUrlItemRepository urlRepository, 
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _urlRepository = urlRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<string> CreateShortUrl(string longUrl)
        {
            if(string.IsNullOrEmpty(longUrl))
            {
                throw new ArgumentNullException(nameof(longUrl));
            }
            if(longUrl.Length > 200)
            {
                throw new InvalidArgumentException("URL is too long");
            }
            if (!longUrl.StartsWith("http://") && !longUrl.StartsWith("https://"))
            {
                throw new InvalidArgumentException("Invalid URL");
            }
            var existedURL = await _urlRepository.GetByUrlAsync(longUrl);
            if (existedURL != null)
            {
                throw new InvalidArgumentException("This URL already already taken");
            }

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in this token");
            }

            string shortCode, shortUrl;
            using (var sha256 = SHA256.Create())
            {
                var sb = new StringBuilder();
                byte[] result = sha256.ComputeHash(Encoding.UTF8.GetBytes(longUrl));
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
                shortCode = sb.ToString().Substring(sb.Length - 6);
                string baseUrl = _configuration.GetValue<string>("Kestrel:Endpoints:Http:Url");
                shortUrl = $"{baseUrl}/{shortCode}";
            }
            var newItem = new UrlItem
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl,
                CreatedByUserId = userId,
                TimeStamp = DateTime.UtcNow,
            };

            var createdItem = await _urlRepository.AddAsync(newItem);
            return createdItem.ShortUrl;
        }
        public async Task<string> GetLongUrl(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                throw new ArgumentNullException(nameof(shortCode));
            }
            if (shortCode.Length > 20)
            {
                throw new InvalidArgumentException("Short URL is too long");
            }
            var shortUrl = "http://localhost:5000/" + shortCode;
            var item = await _urlRepository.GetByUrlAsync(shortCode);
            if (item == null)
            {
                throw new NotFoundException($"URL not found for url {shortUrl}");
            }
            return item.LongUrl;
        }
    }
}
