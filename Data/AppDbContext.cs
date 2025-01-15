using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForMiraiProject.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;

        public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // DbSet Properties
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Hotel> Hotels { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("User")
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Hotel entity configuration
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room entity configuration
            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            modelBuilder.Entity<Room>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Booking entity configuration
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            modelBuilder.Entity<Booking>()
                .Property(b => b.UpdatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Coupon entity configuration
            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountPercentage)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<Coupon>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            modelBuilder.Entity<Coupon>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");
        }

        // Hook for validation before saving changes
        private void OnSavingChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IValidatable validatable)
                    {
                        validatable.Validate();
                    }
                }
            }
        }

        // SaveChanges Override for Validation, Logging, and Error Handling
        public override int SaveChanges()
        {
            try
            {
                OnSavingChanges(); // Perform validation before saving
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes.");
                throw new InvalidOperationException("An error occurred while saving changes.", ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                OnSavingChanges(); // Perform validation before saving
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes asynchronously.");
                throw new InvalidOperationException("An error occurred while saving changes asynchronously.", ex);
            }
        }

        // Security Consideration: Enforce connection string encryption
        public static string GetEncryptedConnectionString(string rawConnectionString)
        {
            // Implement logic for decrypting or securely handling the connection string here.
            return rawConnectionString; // Replace this with the decrypted string.
        }
    }

    // Interface for validation
    public interface IValidatable
    {
        void Validate();
    }
}
