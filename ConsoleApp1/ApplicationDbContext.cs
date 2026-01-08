using AutoPartsWarehouse.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Part> Parts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<SupplyPosition> SupplyPositions { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SalePosition> SalePositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка точности для денег, чтобы SQL не выдавал предупреждений
            modelBuilder.Entity<Part>().Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Part>().Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SupplyPosition>().Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalePosition>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}