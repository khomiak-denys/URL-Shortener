using URL_Shortener.Application.Interfaces.Services;
using URL_Shortener.Application.Interfaces.Repositories;
using URL_Shortener.Application.Dto.User;
using URL_Shortener.Domain.Entities;
using URL_Shortener.Application.Exceptions;

namespace URL_Shortener.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService
            )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginUserDto user)
        {
            user.Password = user.Password.Trim();
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser == null)
            {
                throw new NotFoundException($"User with login {user.Login} not found");
            }
            if (_passwordHasher.VerifyPassword(user.Password, existingUser.PasswordHash))
            {
                return await _tokenService.GetToken(existingUser.Id, existingUser.Login, existingUser.Role);
            }
            else
            {
                throw new InvalidArgumentException("Invalid credentials");
            }
        }
        public async Task<UserDto> RegisterAsync(RegisterUserDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var existringUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existringUser != null)
            {
                throw new AlreadyExistsException("User with this login already exists");
            }
            var newUser = new User
            {
                Login = user.Login,
                PasswordHash = _passwordHasher.HashPassword(user.Password),
                Role = "User"
            };
            var createdUser = await _userRepository.AddAsync(newUser);
            return new UserDto
            {
                Id = createdUser.Id,
                Login = createdUser.Login,
                Role = createdUser.Role
            };
        }
    }
}
