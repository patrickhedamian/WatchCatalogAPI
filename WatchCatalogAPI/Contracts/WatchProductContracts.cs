using System.ComponentModel.DataAnnotations;

namespace WatchCatalogAPI.Contracts;

/// <summary>Fields required to create a watch.</summary>
public sealed record CreateWatchProductRequest
{
    [Required]
    public required string ProductNumber { get; init; }

    [Required]
    public required string ProductName { get; init; }

    [Required]
    public required string Model { get; init; }

    [Required]
    public required string ShortDescription { get; init; }

    [Required]
    public required string FullDescription { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Price { get; init; }

    [Required, Url]
    public required string ImageUrl { get; init; }

    [Required]
    public required string CaliberAndMovement { get; init; }

    public bool Chronograph { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Weight { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Height { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Diameter { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Thickness { get; init; }

    [Range(0, int.MaxValue)]
    public int Jewel { get; init; }

    [Required]
    public required string CaseMaterial { get; init; }

    [Required]
    public required string StrapMaterial { get; init; }

    public bool? Featured { get; init; }
}

/// <summary>Fields used to replace all editable values of a watch.</summary>
public sealed record UpdateWatchProductRequest
{
    [Required]
    public required string ProductNumber { get; init; }

    [Required]
    public required string ProductName { get; init; }

    [Required]
    public required string Model { get; init; }

    [Required]
    public required string ShortDescription { get; init; }

    [Required]
    public required string FullDescription { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Price { get; init; }

    [Required, Url]
    public required string ImageUrl { get; init; }

    [Required]
    public required string CaliberAndMovement { get; init; }

    public bool Chronograph { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Weight { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Height { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Diameter { get; init; }

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Thickness { get; init; }

    [Range(0, int.MaxValue)]
    public int Jewel { get; init; }

    [Required]
    public required string CaseMaterial { get; init; }

    [Required]
    public required string StrapMaterial { get; init; }

    public bool? Featured { get; init; }
}

/// <summary>Watch data returned by the catalog API. Prices are in PHP.</summary>
public sealed record WatchProductResponse(
    int Id,
    string ProductNumber,
    string ProductName,
    string Model,
    string ShortDescription,
    string FullDescription,
    decimal Price,
    string Currency,
    string ImageUrl,
    string CaliberAndMovement,
    bool Chronograph,
    decimal Weight,
    decimal Height,
    decimal Diameter,
    decimal Thickness,
    int Jewel,
    string CaseMaterial,
    string StrapMaterial,
    bool? Featured);
