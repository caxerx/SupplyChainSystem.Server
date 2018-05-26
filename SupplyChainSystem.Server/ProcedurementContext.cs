using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class ProcedurementContext : DbContext
    {
        public ProcedurementContext(DbContextOptions<ProcedurementContext> options) : base(options)
        {
        }

        public DbSet<User> User { set; get; }
        public DbSet<Supplier> Supplier { set; get; }
        public DbSet<Category> Category { set; get; }
        public DbSet<Item> Item { set; get; }
        public DbSet<VirtualIdMap> VirtualIdMap { set; get; }
        public DbSet<VirtualItem> VirtualItem { set; get; }
        public DbSet<CategoryItem> CategoryItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VirtualIdMap>().HasKey(sc => new {sc.SupplierItemId, sc.VirtualItemId});
            modelBuilder.Entity<CategoryItem>().HasKey(sc => new {sc.VirtualItemId, sc.CategoryId});
            modelBuilder.Entity<Item>().HasIndex(sc => sc.SupplierItemId).IsUnique();
            modelBuilder.Entity<VirtualItem>().HasIndex(sc => sc.VirtualItemId).IsUnique();
        }
    }
}