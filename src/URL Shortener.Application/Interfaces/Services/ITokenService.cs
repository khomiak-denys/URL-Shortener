using URL_Shortener.Application.Dto.User;
using URL_Shortener.Domain.Entities;

namespace URL_Shortener.Application.Interfaces.Services
{
    public interface ITokenService
    {
        public Task<LoginResponse> GetToken(long id, string login, string role);
    }
}