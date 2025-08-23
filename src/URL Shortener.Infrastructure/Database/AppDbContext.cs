using URL_Shortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace URL_Shortener.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users {  get; set; }
        public DbSet<UrlItem> UrlItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .HasDatabaseName("IX_UsersLogin")
                .IsUnique();

            modelBuilder.Entity<User>().HasData([
                new User {
                    Id = 1,
                    Login = "Admin",
                    PasswordHash = "$2b$12$RJ3TZExyvnfqQRPseHeli.rJT72KMBgYG9liC9ZdCQjpKIhuP0h5u",
                    Role = "Admin"
                }
                ]);

            modelBuilder.Entity<UrlItem>()
                .HasIndex(i => i.LongUrl)
                .HasDatabaseName("IX_UrlItemsLongUrl")
                .IsUnique();

            modelBuilder.Entity<UrlItem>()
                .HasIndex(i => i.ShortUrl)
                .HasDatabaseName("IX_UrlItemsShortUrl")
                .IsUnique();
        }
    }
}
