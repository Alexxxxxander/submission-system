using Microsoft.EntityFrameworkCore;
using FormSubmissionSystem.Infrastructure.Data.Configurations;

namespace FormSubmissionSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FormSubmissionEntity> Submissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FormSubmissionConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

