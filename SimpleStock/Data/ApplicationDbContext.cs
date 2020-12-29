using Microsoft.EntityFrameworkCore;
using SimpleStock.Data.Model;

namespace SimpleStock.Data
{
    public class ApplicationDbContext
        : DbContext
    {
        public DbSet<StockItem> Items { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<StockItem>()
                .HasIndex(a => a.ID);

            modelBuilder
                .Entity<StockItem>()
                .HasIndex(a => new { a.Manufacturer, a.ProductNumber });

            modelBuilder
                .Entity<Transaction>()
                .HasIndex(a => a.Date);

            base.OnModelCreating(modelBuilder);
        }
    }
}
