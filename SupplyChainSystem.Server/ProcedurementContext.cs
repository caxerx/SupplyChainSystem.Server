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
        public DbSet<Agreement> Agreement { get; set; }
        public DbSet<BlanketPurchaseAgreementDetails> BlanketPurchaseAgreementDetails { get; set; }
        public DbSet<BlanketPurchaseAgreementLine> BlanketPurchaseAgreementLine { get; set; }
        public DbSet<ContractPurchaseAgreementDetails> ContractPurchaseAgreementDetails { get; set; }
        public DbSet<ContractPurchaseAgreementLine> ContractPurchaseAgreementLine { get; set; }
        public DbSet<PlannedPurchaseAgreementDetails> PlannedPurchaseAgreementDetails { get; set; }
        public DbSet<PlannedPurchaseAgreementLine> PlannedPurchaseAgreementLine { get; set; }
        public DbSet<RequestMap> RequestMap { get; set; }
        public DbSet<BlanketRelease> BlanketRelease { get; set; }
        public DbSet<BlanketReleaseLine> BlanketReleaseLine { get; set; }
        public DbSet<DespatchInstruction> DespatchInstruction { get; set; }
        public DbSet<StandardPurchaseOrder> StandardPurchaseOrder { get; set; }
        public DbSet<StandardPurchaseOrderLine> StandardPurchaseOrderLine { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasIndex(sc => sc.SupplierItemId).IsUnique();
            modelBuilder.Entity<VirtualItem>().HasIndex(sc => sc.VirtualItemId).IsUnique();
            modelBuilder.Entity<User>().HasIndex(sc => sc.UserName).IsUnique();
            modelBuilder.Entity<VirtualIdMap>().HasKey(sc => new {sc.ItemId, sc.VirtualItemId});
            modelBuilder.Entity<CategoryItem>().HasKey(sc => new {sc.VirtualItemId, sc.CategoryId});
            modelBuilder.Entity<RequestItem>().HasKey(sc => new {sc.RequestId, sc.VirtualItemId});
            modelBuilder.Entity<StockItem>().HasKey(sc => new {sc.StockId, sc.VirtualItemId});
            modelBuilder.Entity<BlanketPurchaseAgreementLine>().HasKey(sc => new {sc.AgreementId, sc.ItemId});
            modelBuilder.Entity<ContractPurchaseAgreementLine>().HasKey(sc => new {sc.AgreementId, sc.ItemId});
            modelBuilder.Entity<PlannedPurchaseAgreementLine>().HasKey(sc => new {sc.AgreementId, sc.ItemId});
        }
    }
}