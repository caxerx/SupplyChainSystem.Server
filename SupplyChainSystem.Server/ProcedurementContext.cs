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
        public DbSet<RestaurantType> RestaurantType { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockItem> StockItem { get; set; }
        public DbSet<Restaurant> Restaurant { get; set; }
        public DbSet<RestaurantManager> RestaurantManager { get; set; }
        public DbSet<Models.Request> Request { get; set; }
        public DbSet<RequestItem> RequestItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasIndex(sc => sc.SupplierItemId).IsUnique();
            modelBuilder.Entity<VirtualItem>().HasIndex(sc => sc.VirtualItemId).IsUnique();
            modelBuilder.Entity<User>().HasIndex(sc => sc.UserName).IsUnique();
            modelBuilder.Entity<VirtualIdMap>().HasKey(sc => new {sc.ItemId, sc.VirtualItemId});
            modelBuilder.Entity<CategoryItem>().HasKey(sc => new {sc.VirtualItemId, sc.CategoryId});
            modelBuilder.Entity<RequestItem>().HasKey(sc => new {sc.RequestId, sc.VirtualItemId});
            modelBuilder.Entity<StockItem>().HasKey(sc => new {sc.StockId, sc.VirtualItemId});
        }
    }
}