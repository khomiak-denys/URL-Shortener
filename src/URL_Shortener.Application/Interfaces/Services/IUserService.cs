using URL_Shortener.Application.Dto.User;


namespace URL_Shortener.Application.Interfaces.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> LoginAsync(LoginUserDto user);
        public Task<UserDto> RegisterAsync(RegisterUserDto user);
    }
}
