using System.ComponentModel.DataAnnotations;

namespace URL_Shortener.Application.Dto.User
{
    public class LoginUserDto
    {
        [Required]
        public string Login { get; set; }
        [Required]
        [MaxLength(10)]
        public string Password { get; set; }
    }
}