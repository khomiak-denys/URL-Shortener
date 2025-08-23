
namespace URL_Shortener.Application.Dto.User
{
    public class LoginResponse
    {
        public string? Login { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}