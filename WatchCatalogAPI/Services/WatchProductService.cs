using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WatchCatalogAPI.Contracts;
using WatchCatalogAPI.Data;
using WatchCatalogAPI.Models;

namespace WatchCatalogAPI.Services;

public sealed class WatchProductService(
    WatchCatalogDbContext dbContext,
    IMemoryCache cache,
    IWatchImageStorage imageStorage) : IWatchProductService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string ProductListCacheKey = "watch-products:all";

    public async Task<IReadOnlyList<WatchProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(ProductListCacheKey, out IReadOnlyList<WatchProductResponse>? cachedProducts)
            && cachedProducts is not null)
        {
            return cachedProducts;
        }

        var products = await dbContext.Watches
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(product => new WatchProductResponse(
                product.Id,
                product.ProductNumber,
                product.ProductName,
                product.Model,
                product.ShortDescription,
                product.FullDescription,
                product.Price,
                "PHP",
                product.ImageUrl,
                product.CaliberAndMovement,
                product.Chronograph,
                product.Weight,
                product.Height,
                product.Diameter,
                product.Thickness,
                product.Jewel,
                product.CaseMaterial,
                product.StrapMaterial,
                product.Featured))
            .ToListAsync(cancellationToken);

        cache.Set(ProductListCacheKey, products, CacheDuration);
        return products;
    }

    public async Task<WatchProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = GetProductCacheKey(id);
        if (cache.TryGetValue(cacheKey, out WatchProductResponse? cachedProduct))
        {
            return cachedProduct;
        }

        var product = await dbContext.Watches
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new WatchProductResponse(
                product.Id,
                product.ProductNumber,
                product.ProductName,
                product.Model,
                product.ShortDescription,
                product.FullDescription,
                product.Price,
                "PHP",
                product.ImageUrl,
                product.CaliberAndMovement,
                product.Chronograph,
                product.Weight,
                product.Height,
                product.Diameter,
                product.Thickness,
                product.Jewel,
                product.CaseMaterial,
                product.StrapMaterial,
                product.Featured))
            .SingleOrDefaultAsync(cancellationToken);

        if (product is not null)
        {
            cache.Set(cacheKey, product, CacheDuration);
        }

        return product;
    }

    public async Task<WatchProductResponse> CreateAsync(
        CreateWatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new WatchProduct
        {
            ProductNumber = request.ProductNumber.Trim(),
            ProductName = request.ProductName.Trim(),
            Model = request.Model.Trim(),
            ShortDescription = request.ShortDescription.Trim(),
            FullDescription = request.FullDescription.Trim(),
            Price = request.Price,
            CaliberAndMovement = request.CaliberAndMovement.Trim(),
            Chronograph = request.Chronograph,
            Weight = request.Weight,
            Height = request.Height,
            Diameter = request.Diameter,
            Thickness = request.Thickness,
            Jewel = request.Jewel,
            CaseMaterial = request.CaseMaterial.Trim(),
            StrapMaterial = request.StrapMaterial.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            Featured = request.Featured
        };

        dbContext.Watches.Add(product);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlServerUniqueConstraint.IsViolation(exception))
        {
            throw new ProductNumberConflictException();
        }

        InvalidateProductCaches(product.Id);
        return ToResponse(product);
    }

    public async Task<WatchProductResponse?> UpdateAsync(
        int id,
        UpdateWatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Watches
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var previousImageUrl = product.ImageUrl;
        product.ProductNumber = request.ProductNumber.Trim();
        product.ProductName = request.ProductName.Trim();
        product.Model = request.Model.Trim();
        product.ShortDescription = request.ShortDescription.Trim();
        product.FullDescription = request.FullDescription.Trim();
        product.Price = request.Price;
        product.CaliberAndMovement = request.CaliberAndMovement.Trim();
        product.Chronograph = request.Chronograph;
        product.Weight = request.Weight;
        product.Height = request.Height;
        product.Diameter = request.Diameter;
        product.Thickness = request.Thickness;
        product.Jewel = request.Jewel;
        product.CaseMaterial = request.CaseMaterial.Trim();
        product.StrapMaterial = request.StrapMaterial.Trim();
        product.ImageUrl = request.ImageUrl.Trim();
        product.Featured = request.Featured;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlServerUniqueConstraint.IsViolation(exception))
        {
            throw new ProductNumberConflictException();
        }

        InvalidateProductCaches(id);

        if (!string.Equals(previousImageUrl, product.ImageUrl, StringComparison.Ordinal)
            && !await dbContext.Watches.AnyAsync(
                other => other.Id != id && other.ImageUrl == previousImageUrl,
                cancellationToken))
        {
            await imageStorage.DeleteIfManagedAsync(previousImageUrl, cancellationToken);
        }

        return ToResponse(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Watches
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        if (!await dbContext.Watches.AnyAsync(
                other => other.Id != id && other.ImageUrl == product.ImageUrl,
                cancellationToken))
        {
            await imageStorage.DeleteIfManagedAsync(product.ImageUrl, cancellationToken);
        }

        dbContext.Watches.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        InvalidateProductCaches(id);
        return true;
    }

    public async Task<WatchProductResponse?> ReplaceImageAsync(
        int id,
        IFormFile image,
        string contentType,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Watches
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var previousImageUrl = product.ImageUrl;
        await using var imageStream = image.OpenReadStream();
        product.ImageUrl = await imageStorage.UploadAsync(
            imageStream,
            contentType,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (!await dbContext.Watches.AnyAsync(
                other => other.Id != id && other.ImageUrl == previousImageUrl,
                cancellationToken))
        {
            await imageStorage.DeleteIfManagedAsync(previousImageUrl, cancellationToken);
        }

        InvalidateProductCaches(id);
        return ToResponse(product);
    }

    private void InvalidateProductCaches(int id)
    {
        cache.Remove(ProductListCacheKey);
        cache.Remove(GetProductCacheKey(id));
    }

    private static string GetProductCacheKey(int id) => $"watch-products:{id}";

    private static WatchProductResponse ToResponse(WatchProduct product) =>
        new(
            product.Id,
            product.ProductNumber,
            product.ProductName,
            product.Model,
            product.ShortDescription,
            product.FullDescription,
            product.Price,
            "PHP",
            product.ImageUrl,
            product.CaliberAndMovement,
            product.Chronograph,
            product.Weight,
            product.Height,
            product.Diameter,
            product.Thickness,
            product.Jewel,
            product.CaseMaterial,
            product.StrapMaterial,
            product.Featured);
}

public sealed class ProductNumberConflictException()
    : Exception("A watch with this product number already exists.");

internal static class SqlServerUniqueConstraint
{
    public static bool IsViolation(DbUpdateException exception) =>
        exception.InnerException is Microsoft.Data.SqlClient.SqlException
        {
            Number: 2601 or 2627
        };
}
