using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Model> Model { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<ReviewItem> ReviewItems { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<RentDevice> RentDevices { get; set; }
        public DbSet<Rental> Rental { get; set; }

        public DbSet<Scale> Scales { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ReviewItem>().HasKey(x => new { x.Id });
            builder.Entity<Review>().HasKey(x => new { x.ReviewId });
            builder.Entity<Model>().HasKey(x => new { x.Id });
            builder.Entity<Device>().HasKey(x => new { x.Id });
            builder.Entity<RentDevice>().HasKey(x => new { x.DeviceId });
            builder.Entity<Rental>().HasKey(x => new { x.Id });

            builder.Entity<Scale>().HasKey(x => x.Id);
            builder.Entity<Repair>().HasKey(x => x.Id);
            builder.Entity<Receipt>().HasKey(x => x.Id);
            builder.Entity<ReceiptItem>().HasKey(x => x.Id);

            builder.Entity<Repair>()
                .HasOne(r => r.Scale)
                .WithMany(s => s.Repairs)
                .HasForeignKey(r => r.ScaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReceiptItem>()
                .HasOne(ri => ri.Repair)
                .WithMany(r => r.ReceiptItems)
                .HasForeignKey(ri => ri.RepairId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReceiptItem>()
                .HasOne(ri => ri.Receipt)
                .WithMany(r => r.ReceiptItems)
                .HasForeignKey(ri => ri.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}