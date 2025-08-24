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
            var shortenURL = await _urlService.CreateShortUrlAsync(longUrl);
            return Ok(shortenURL);
        }

        [AllowAnonymous]
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetLongURL(string shortCode)
        {
            var longUrl = await _urlService.GetLongUrlAsync(shortCode);
            return Redirect(longUrl);
        }

        [HttpGet("url-list")]
        public async Task<IActionResult> GetAll()
        {
            var urlList = await _urlService.GetAllAsync();
            return Ok(urlList);
        }

        [Authorize(Roles = "Admin, User")]

        [HttpGet("url/{id}/details")]
        public async Task<IActionResult> GetById(long id)
        {
            var url = await _urlService.GetByIdAsync(id);
            return Ok(url);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("url/{id}")]
        public async Task<IActionResult> DeleteByIdAsync(long id)
        {
            await _urlService.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
