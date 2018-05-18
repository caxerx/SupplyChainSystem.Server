using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class ProcedurementContext : DbContext
    {
        public DbSet<User> Users { set; get; }

        public ProcedurementContext(DbContextOptions<ProcedurementContext> options) : base(options)
        {

        }

    }
}