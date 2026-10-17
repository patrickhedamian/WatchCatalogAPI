namespace WatchCatalogAPI.Models;

public sealed class WatchProduct
{
    public int Id { get; set; }
    public required string ProductNumber { get; set; }
    public required string ProductName { get; set; }
    public required string Model { get; set; }
    public required string ShortDescription { get; set; }
    public required string FullDescription { get; set; }
    public decimal Price { get; set; }
    public required string ImageUrl { get; set; }
    public required string CaliberAndMovement { get; set; }
    public bool Chronograph { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Diameter { get; set; }
    public decimal Thickness { get; set; }
    public int Jewel { get; set; }
    public required string CaseMaterial { get; set; }
    public required string StrapMaterial { get; set; }
    public bool? Featured { get; set; }
    public byte[]? ImageBlob { get; set; }
}
