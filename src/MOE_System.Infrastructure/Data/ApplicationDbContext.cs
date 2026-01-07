using Microsoft.EntityFrameworkCore;

namespace MOE_System.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSet properties for your entities here
    // Example: public DbSet<Entity> Entities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity relationships and constraints here
        // Example: modelBuilder.Entity<Entity>().HasKey(e => e.Id);
    }
}
