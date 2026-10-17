using Microsoft.EntityFrameworkCore;
using WatchCatalogAPI.Models;

namespace WatchCatalogAPI.Data;

public sealed class WatchCatalogDbContext(DbContextOptions<WatchCatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<WatchProduct> WatchProducts => Set<WatchProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<WatchProduct>();
        product.HasKey(watch => watch.Id);
        product.HasIndex(watch => watch.ProductNumber).IsUnique();
        product.Property(watch => watch.ProductNumber).HasMaxLength(50).IsRequired();
        product.Property(watch => watch.Name).HasMaxLength(150).IsRequired();
        product.Property(watch => watch.Model).HasMaxLength(100).IsRequired();
        product.Property(watch => watch.ShortDescription).HasMaxLength(300).IsRequired();
        product.Property(watch => watch.FullDescription).HasMaxLength(4000).IsRequired();
        product.Property(watch => watch.Price).HasPrecision(18, 2);
        product.Property(watch => watch.CaliberAndMovement).HasMaxLength(200).IsRequired();
        product.Property(watch => watch.WeightGrams).HasPrecision(10, 2);
        product.Property(watch => watch.HeightMm).HasPrecision(10, 2);
        product.Property(watch => watch.DiameterMm).HasPrecision(10, 2);
        product.Property(watch => watch.ThicknessMm).HasPrecision(10, 2);
        product.Property(watch => watch.CaseMaterial).HasMaxLength(100).IsRequired();
        product.Property(watch => watch.StrapMaterial).HasMaxLength(100).IsRequired();
        product.Property(watch => watch.ImageUrl).HasMaxLength(2048).IsRequired();

        product.HasData(
            Seed(1, "WC-1001", "Aster", "Field Automatic", "A versatile automatic field watch.", "A brushed steel field watch with a clear dial, luminous hands, and dependable automatic movement.", 12900m, "Automatic, 3 hands", false, 82m, 11m, 40m, 11m, 24, "316L stainless steel", "Canvas"),
            Seed(2, "WC-1002", "Meridian", "Chronograph 42", "A precise everyday chronograph.", "A balanced chronograph with three subdials, a tachymeter scale, and a comfortable stainless-steel bracelet.", 18500m, "Quartz chronograph", true, 145m, 12m, 42m, 12m, 0, "Stainless steel", "Stainless steel"),
            Seed(3, "WC-1003", "Northstar", "Diver 200", "A bold watch for water and land.", "A robust dive-style watch with a rotating timing bezel, high-contrast dial, and 200-metre water resistance.", 24750m, "Automatic, 3 hands", false, 168m, 13m, 41m, 13m, 21, "316L stainless steel", "Rubber"),
            Seed(4, "WC-1004", "Solstice", "Moonphase Classic", "A refined dress watch with moonphase.", "A slim dress watch featuring a moonphase display, applied hour markers, and a polished stainless-steel case.", 32900m, "Automatic, moonphase", false, 74m, 9m, 39m, 9m, 26, "Stainless steel", "Genuine leather"),
            Seed(5, "WC-1005", "Vector", "Pilot GMT", "Track a second time zone at a glance.", "A legible pilot-inspired GMT watch with a 24-hour hand, oversized crown, and a durable nylon strap.", 21800m, "Automatic GMT", false, 91m, 12m, 42m, 12m, 24, "Titanium", "Nylon"),
            Seed(6, "WC-1006", "Harbor", "Small Seconds", "A clean dial with a small-seconds display.", "A classic small-seconds watch with a textured dial, slim profile, and versatile brown leather strap.", 16250m, "Manual winding, small seconds", false, 68m, 8m, 38m, 8m, 17, "Stainless steel", "Leather"),
            Seed(7, "WC-1007", "Summit", "Sport Solar", "Low-maintenance solar-powered performance.", "A lightweight sports watch with solar charging, luminous markers, and a scratch-resistant mineral crystal.", 7950m, "Solar quartz, 3 hands", false, 56m, 10m, 40m, 10m, 0, "Resin", "Resin"),
            Seed(8, "WC-1008", "Atelier", "Open Balance", "An open-heart view of the movement.", "An elegant automatic watch with an open-heart window, exhibition case back, and a textured leather strap.", 28400m, "Automatic, open-heart", false, 88m, 11m, 40m, 11m, 20, "Stainless steel", "Leather"),
            Seed(9, "WC-1009", "Tide", "Coastal Diver", "A compact diver-inspired daily watch.", "A compact diver-inspired model with a unidirectional bezel, luminous display, and a flexible silicone strap.", 13900m, "Automatic, 3 hands", false, 119m, 12m, 38m, 12m, 24, "Stainless steel", "Silicone"),
            Seed(10, "WC-1010", "Lumen", "Minimal 38", "A pared-back design for everyday wear.", "A minimalist three-hand watch with a clean dial, slim case, and quick-release mesh bracelet.", 9900m, "Quartz, 3 hands", false, 96m, 7m, 38m, 7m, 0, "Stainless steel", "Mesh steel"));
    }

    private static WatchProduct Seed(
        int id,
        string productNumber,
        string name,
        string model,
        string shortDescription,
        string fullDescription,
        decimal price,
        string caliberAndMovement,
        bool chronograph,
        decimal weightGrams,
        decimal heightMm,
        decimal diameterMm,
        decimal thicknessMm,
        int jewelCount,
        string caseMaterial,
        string strapMaterial) =>
        new()
        {
            Id = id,
            ProductNumber = productNumber,
            Name = name,
            Model = model,
            ShortDescription = shortDescription,
            FullDescription = fullDescription,
            Price = price,
            CaliberAndMovement = caliberAndMovement,
            Chronograph = chronograph,
            WeightGrams = weightGrams,
            HeightMm = heightMm,
            DiameterMm = diameterMm,
            ThicknessMm = thicknessMm,
            JewelCount = jewelCount,
            CaseMaterial = caseMaterial,
            StrapMaterial = strapMaterial,
            ImageUrl = $"https://placehold.co/640x480?text={Uri.EscapeDataString(name + " " + model)}"
        };
}
