using Microsoft.AspNetCore.Mvc;
using URL_Shortener.Application.Interfaces.Services;
using URL_Shortener.Application.Dto.User;


namespace URL_Shortener.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginUserDto user)
        {
            var loggedUser = await _userService.LoginAsync(user); 
            return Ok(loggedUser);
        } 

        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterUserDto user)
        {
            var registeredUser = await _userService.RegisterAsync(user);
            return Ok(registeredUser);
        }
    }
}
