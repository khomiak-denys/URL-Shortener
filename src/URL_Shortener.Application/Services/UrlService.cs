using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using URL_Shortener.Application.Exceptions;
using URL_Shortener.Application.Interfaces.Repositories;
using URL_Shortener.Application.Interfaces.Services;
using URL_Shortener.Application.Dto.Url;
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

        public async Task<string> CreateShortUrlAsync(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
            {
                throw new ArgumentNullException(nameof(longUrl));
            }
            if (longUrl.Length > 200)
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

                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

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
        public async Task<string> GetLongUrlAsync(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                throw new ArgumentNullException(nameof(shortCode));
            }
            if (shortCode.Length > 20)
            {
                throw new InvalidArgumentException("Short URL is too long");
            }
            var baseUrl = _configuration.GetValue<string>("Kestrel:Endpoints:Http:Url");
            var shortUrl = $"{baseUrl}/{shortCode}";
            var item = await _urlRepository.GetByUrlAsync(shortCode);
            if (item == null)
            {
                throw new NotFoundException($"Long url not found for short url {shortUrl}");
            }
            return item.LongUrl;
        }

        public async Task<IEnumerable<UrlListDto>> GetAllAsync()
        {
            var urlList = await _urlRepository.GetAllAsync();
            return urlList.Select(i => new UrlListDto
            {
                Id = i.Id,
                LongUrl = i.LongUrl,
                ShortUrl = i.ShortUrl
            });
        }
        public async Task<UrlDetailsDto> GetByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new InvalidArgumentException("Invalid id");
            }
            var url = await _urlRepository.GetByIdAsync(id);
            if (url == null)
            {
                throw new NotFoundException($"Url with id {id} not found");
            }
            return new UrlDetailsDto
            {
                Id = url.Id,
                LongUrl = url.LongUrl,
                ShortUrl = url.ShortUrl,
                CreatedByUserId = url.CreatedByUserId,
                Timestamp = url.TimeStamp
            };
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new InvalidArgumentException("Invalid id");
            }
            var existedItem = await _urlRepository.GetByIdAsync(id);
            if (existedItem == null)
            {
                throw new NotFoundException($"Item with id {id} not found");
            }

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in this token");
            }

            var userRoleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (String.IsNullOrEmpty(userRoleClaim))
            {
                throw new UnauthorizedAccessException("User role not found in this token");
            }
            if (userId == existedItem.CreatedByUserId || userRoleClaim == "Admin")
            {
                return await _urlRepository.DeleteById(id);
            } else
            {
                throw new MethodAccessException("Method not allowed");
            }

            
        }
    }
}
