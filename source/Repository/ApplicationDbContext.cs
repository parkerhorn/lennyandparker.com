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
            //TODO Add requirements for name and address

            entity.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.DietaryRestrictions)
                .HasMaxLength(500);
        });
    }
} 