using CommonLib.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonLib.Repository;

public class CommonDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public CommonDbContext(DbContextOptions<CommonDbContext> dbContextOption) : base(dbContextOption)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.UserId)
            .ValueGeneratedNever();
    }
}
