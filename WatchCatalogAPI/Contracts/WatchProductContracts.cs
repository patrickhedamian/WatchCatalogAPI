using System.ComponentModel.DataAnnotations;

namespace WatchCatalogAPI.Contracts;

/// <summary>Fields required to create a watch product.</summary>
public sealed record CreateWatchProductRequest
{
    [Required, StringLength(50)]
    public required string ProductNumber { get; init; }

    [Required, StringLength(150)]
    public required string Name { get; init; }

    [Required, StringLength(100)]
    public required string Model { get; init; }

    [Required, StringLength(300)]
    public required string ShortDescription { get; init; }

    [Required, StringLength(4000)]
    public required string FullDescription { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Price { get; init; }

    [Required, StringLength(200)]
    public required string CaliberAndMovement { get; init; }

    public bool Chronograph { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal WeightGrams { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal HeightMm { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal DiameterMm { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal ThicknessMm { get; init; }

    [Range(0, int.MaxValue)]
    public int JewelCount { get; init; }

    [Required, StringLength(100)]
    public required string CaseMaterial { get; init; }

    [Required, StringLength(100)]
    public required string StrapMaterial { get; init; }

    [Required, Url, StringLength(2048)]
    public required string ImageUrl { get; init; }
}

/// <summary>Fields used to replace all editable values of a watch product.</summary>
public sealed record UpdateWatchProductRequest
{
    [Required, StringLength(50)]
    public required string ProductNumber { get; init; }

    [Required, StringLength(150)]
    public required string Name { get; init; }

    [Required, StringLength(100)]
    public required string Model { get; init; }

    [Required, StringLength(300)]
    public required string ShortDescription { get; init; }

    [Required, StringLength(4000)]
    public required string FullDescription { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Price { get; init; }

    [Required, StringLength(200)]
    public required string CaliberAndMovement { get; init; }

    public bool Chronograph { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal WeightGrams { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal HeightMm { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal DiameterMm { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal ThicknessMm { get; init; }

    [Range(0, int.MaxValue)]
    public int JewelCount { get; init; }

    [Required, StringLength(100)]
    public required string CaseMaterial { get; init; }

    [Required, StringLength(100)]
    public required string StrapMaterial { get; init; }

    [Required, Url, StringLength(2048)]
    public required string ImageUrl { get; init; }
}

/// <summary>Watch product data returned by the catalog API. Prices are in PHP.</summary>
public sealed record WatchProductResponse(
    int Id,
    string ProductNumber,
    string Name,
    string Model,
    string ShortDescription,
    string FullDescription,
    decimal Price,
    string Currency,
    string CaliberAndMovement,
    bool Chronograph,
    decimal WeightGrams,
    decimal HeightMm,
    decimal DiameterMm,
    decimal ThicknessMm,
    int JewelCount,
    string CaseMaterial,
    string StrapMaterial,
    string ImageUrl);
