using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class ProcedurementContext : DbContext
    {
        public DbSet<User> User { set; get; }
        public DbSet<Supplier> Supplier { set; get; }
        public DbSet<Item> Item { set; get; }
        public DbSet<VirtualIdMap> VirtualIdMap { set; get; }
        public DbSet<VirtualItem> VirtualItem { set; get; }

        public ProcedurementContext(DbContextOptions<ProcedurementContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VirtualIdMap>().HasKey(sc => new { sc.ItemId, sc.VirtualItemId });
        }


    }
}