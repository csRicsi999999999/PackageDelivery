using Microsoft.EntityFrameworkCore;
using PackageDelivery.Models;

namespace PackageDelivery.Data;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<FragilePackage> FragilePackages => Set<FragilePackage>();
    public DbSet<RefrigeratedPackage> RefrigeratedPackages => Set<RefrigeratedPackage>();
    public DbSet<Trackings> Trackings => Set<Trackings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=packagedelivery.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ügyfél kötelező mezők
        modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired();
        modelBuilder.Entity<Customer>().Property(c => c.Email).IsRequired();
        modelBuilder.Entity<Customer>().Property(c => c.PhoneNumber).IsRequired();

        // 1. A nyomkövetési azonosító legyen egyedi (adatbázis szinten is)
        modelBuilder.Entity<Package>()
            .HasIndex(p => p.TrackingNumber)
            .IsUnique();

        // 2. Feladó kapcsolat
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Sender)
            .WithMany(c => c.SentPackages)
            .HasForeignKey(p => p.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. Címzett kapcsolat
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Recipient)
            .WithMany(c => c.RecipientPackages)
            .HasForeignKey(p => p.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}