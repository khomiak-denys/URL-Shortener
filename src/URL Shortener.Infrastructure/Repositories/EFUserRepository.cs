using URL_Shortener.Application.Interfaces.Repositories;
using URL_Shortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Infrastructure.Database;


namespace URL_Shortener.Infrastructure.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public EFUserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<User> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
