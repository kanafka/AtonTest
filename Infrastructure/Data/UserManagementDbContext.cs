using Domain.Entytis;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Infrastructure.Data;

public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Login).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(200).IsRequired(); // Increased for hashed passwords
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ModifiedBy).HasMaxLength(50).IsRequired();
            entity.Property(e => e.RevokedBy).HasMaxLength(50);
            entity.Property(e => e.Gender).HasConversion<int>();
        });

        base.OnModelCreating(modelBuilder);
    }
}