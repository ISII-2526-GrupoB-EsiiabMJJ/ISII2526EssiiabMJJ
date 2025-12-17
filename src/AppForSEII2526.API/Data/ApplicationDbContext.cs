using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>//DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

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

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ReviewItem>().HasKey(x => x.Id);
            builder.Entity<Review>().HasKey(x => x.ReviewId);
            builder.Entity<Model>().HasKey(x => x.Id);
            builder.Entity<Device>().HasKey(x => x.Id);
            builder.Entity<RentDevice>().HasKey(x => x.DeviceId);
            builder.Entity<Rental>().HasKey(x => x.Id);
            builder.Entity<Scale>().HasKey(x => x.Id);
            builder.Entity<Repair>().HasKey(x => x.Id);
            builder.Entity<Receipt>().HasKey(x => x.Id);
            builder.Entity<ReceiptItem>().HasKey(x => x.Id);

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

            builder.Entity<Repair>()
                .HasOne(r => r.Scale)
                .WithMany(s => s.Repairs)
                .HasForeignKey(r => r.ScaleId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Receipt>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Receipts)
                .HasForeignKey(r => r.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Purchase>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseItem>()
                .HasKey(pi => new { pi.DeviceId, pi.PurchaseId });

            builder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PurchaseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Device)
                .WithMany()
                .HasForeignKey(pi => pi.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewItem>()
                .HasOne(ri => ri.Device)
                .WithMany(d => d.ReviewItems)
                .HasForeignKey(ri => ri.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewItem>()
                .HasOne(ri => ri.Review)
                .WithMany(r => r.ReviewItems)
                .HasForeignKey(ri => ri.ReviewId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


           builder.Entity<RentDevice>()
                .HasKey(rd => new { rd.RentalId, rd.DeviceId });

            builder.Entity<RentDevice>()
                .HasOne(rd => rd.Device)
                .WithMany(d => d.RentedDevices)
                .HasForeignKey(rd => rd.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RentDevice>()
                .HasOne(rd => rd.Rent)
                .WithMany(r => r.RentalItems)
                .HasForeignKey(rd => rd.RentalId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rental>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Device>()
                .HasOne(d => d.Model)
                .WithMany(m => m.Devices)
                .HasForeignKey(d => d.ModelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}