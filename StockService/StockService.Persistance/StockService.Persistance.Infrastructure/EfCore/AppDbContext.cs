using Microsoft.EntityFrameworkCore;
using StockService.Core.Domain.Entities.Inventory;
using StockService.Core.Domain.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Persistance.Infrastructure.EfCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<StockItem> stockItems { get; set; }
        public DbSet<StockCountItem> stockCountItems { get; set; }
        public DbSet<StockCountSession> stockCountSessions { get; set; }
        public DbSet<Location> locations { get; set; }
        public DbSet<Warehouse> warehouses { get; set; }
    }
}
