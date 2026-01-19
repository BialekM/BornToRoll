using BornToRollWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BornToRollWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    }
}
