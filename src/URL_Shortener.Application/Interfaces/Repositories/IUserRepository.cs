using URL_Shortener.Domain.Entities;

namespace URL_Shortener.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetByLoginAsync(string login);
        public Task<User> AddAsync(User user);
    }
}
