using Microsoft.EntityFrameworkCore;
using WeddingAPI.Models;

namespace WeddingAPI.Repository;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RSVP> RSVPs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RSVP>(entity =>
        {
            entity.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(i => i.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(i => i.IsAttending)
                .IsRequired();

            entity.Property(i => i.DietaryRestrictions)
                .HasMaxLength(500);

            entity.Property(i => i.AccessibilityRequirements)
                .HasMaxLength(500);
                
            entity.Property(i => i.Pronouns)
                .HasMaxLength(20);

            entity.Property(i => i.Note)
                .HasMaxLength(500);
        });
    }
} 