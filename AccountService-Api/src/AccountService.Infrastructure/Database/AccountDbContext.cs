using Microsoft.EntityFrameworkCore;
using AccountService.Core.DbModels;

namespace AccountService.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
    }
}
