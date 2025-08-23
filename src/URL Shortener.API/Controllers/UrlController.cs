using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URL_Shortener.Application.Interfaces.Services;


namespace URL_Shortener.API.Controllers
{
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [Authorize]
        [HttpPost("shorten")]
        public async Task<IActionResult> CreateShortURL([FromBody] string longUrl)
        {
            var shortenURL = await _urlService.CreateShortUrl(longUrl);
            return Ok(shortenURL);
        }

        [AllowAnonymous]
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetLongURL(string shortCode)
        {
            var longUrl = await _urlService.GetLongUrl(shortCode);
            return Redirect(longUrl);
        }
    }
}
