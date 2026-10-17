using Microsoft.AspNetCore.Mvc;
using WatchCatalogAPI.Contracts;
using WatchCatalogAPI.Services;

namespace WatchCatalogAPI.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class WatchProductsController(IWatchProductService productService) : ControllerBase
{
    private const long MaximumImageBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedImageTypes =
        ["image/jpeg", "image/png", "image/webp"];

    /// <summary>Returns all watch products.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WatchProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WatchProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await productService.GetAllAsync(cancellationToken));
    }

    /// <summary>Returns a watch product by its ID.</summary>
    [HttpGet("{id:int}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(WatchProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchProductResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>Creates a watch product.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WatchProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WatchProductResponse>> Create(
        CreateWatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Replaces all editable values for a watch product.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WatchProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WatchProductResponse>> Update(
        int id,
        UpdateWatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.UpdateAsync(id, request, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>Deletes a watch product and its unshared managed image.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await productService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>Uploads a new image for a watch product, replacing its current image.</summary>
    [HttpPut("{id:int}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(WatchProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(MaximumImageBytes)]
    public async Task<ActionResult<WatchProductResponse>> ReplaceImage(
        int id,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            return BadRequest("An image file is required.");
        }

        if (image.Length > MaximumImageBytes)
        {
            return BadRequest("The image must be 10 MB or smaller.");
        }

        var contentType = image.ContentType.Split(';', 2)[0].Trim().ToLowerInvariant();
        if (!AllowedImageTypes.Contains(contentType)
            || !await HasExpectedImageSignatureAsync(image, contentType, cancellationToken))
        {
            return BadRequest("The file must be a valid JPEG, PNG, or WebP image.");
        }

        var product = await productService.ReplaceImageAsync(id, image, contentType, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    private static async Task<bool> HasExpectedImageSignatureAsync(
        IFormFile image,
        string contentType,
        CancellationToken cancellationToken)
    {
        var header = new byte[12];
        await using var stream = image.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header, cancellationToken);

        return contentType switch
        {
            "image/jpeg" => bytesRead >= 3
                && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            "image/png" => bytesRead >= 8
                && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            "image/webp" => bytesRead >= 12
                && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
                && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };
    }
}
