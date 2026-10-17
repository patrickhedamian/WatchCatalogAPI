namespace WatchCatalogAPI.Models;

public sealed class WatchProduct
{
    public int Id { get; set; }
    public required string ProductNumber { get; set; }
    public required string Name { get; set; }
    public required string Model { get; set; }
    public required string ShortDescription { get; set; }
    public required string FullDescription { get; set; }
    public decimal Price { get; set; }
    public required string CaliberAndMovement { get; set; }
    public bool Chronograph { get; set; }
    public decimal WeightGrams { get; set; }
    public decimal HeightMm { get; set; }
    public decimal DiameterMm { get; set; }
    public decimal ThicknessMm { get; set; }
    public int JewelCount { get; set; }
    public required string CaseMaterial { get; set; }
    public required string StrapMaterial { get; set; }
    public required string ImageUrl { get; set; }
}
