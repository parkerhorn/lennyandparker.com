using Microsoft.EntityFrameworkCore;
using WeddingAPI.Models;

namespace WeddingAPI.Repository;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Invitation> Invitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.Property(i => i.NumberOfAttendees)
                .HasDefaultValue(1);

            entity.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.DietaryRestrictions)
                .HasMaxLength(500);
        });
    }
} 