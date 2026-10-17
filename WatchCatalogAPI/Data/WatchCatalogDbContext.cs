using Microsoft.EntityFrameworkCore;
using WatchCatalogAPI.Models;

namespace WatchCatalogAPI.Data;

public sealed class WatchCatalogDbContext(DbContextOptions<WatchCatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<WatchProduct> Watches => Set<WatchProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var watch = modelBuilder.Entity<WatchProduct>();
        watch.ToTable("Watches");
        watch.HasKey(item => item.Id);
        watch.Property(item => item.ImageBlob).HasColumnType("varbinary(max)");
    }
}
