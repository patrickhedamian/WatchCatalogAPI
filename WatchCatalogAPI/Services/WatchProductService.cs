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

        var products = await dbContext.WatchProducts
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(product => ToResponse(product))
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

        var product = await dbContext.WatchProducts
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => ToResponse(product))
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
            Name = request.Name.Trim(),
            Model = request.Model.Trim(),
            ShortDescription = request.ShortDescription.Trim(),
            FullDescription = request.FullDescription.Trim(),
            Price = request.Price,
            CaliberAndMovement = request.CaliberAndMovement.Trim(),
            Chronograph = request.Chronograph,
            WeightGrams = request.WeightGrams,
            HeightMm = request.HeightMm,
            DiameterMm = request.DiameterMm,
            ThicknessMm = request.ThicknessMm,
            JewelCount = request.JewelCount,
            CaseMaterial = request.CaseMaterial.Trim(),
            StrapMaterial = request.StrapMaterial.Trim(),
            ImageUrl = request.ImageUrl.Trim()
        };

        dbContext.WatchProducts.Add(product);
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
        var product = await dbContext.WatchProducts
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var previousImageUrl = product.ImageUrl;
        product.ProductNumber = request.ProductNumber.Trim();
        product.Name = request.Name.Trim();
        product.Model = request.Model.Trim();
        product.ShortDescription = request.ShortDescription.Trim();
        product.FullDescription = request.FullDescription.Trim();
        product.Price = request.Price;
        product.CaliberAndMovement = request.CaliberAndMovement.Trim();
        product.Chronograph = request.Chronograph;
        product.WeightGrams = request.WeightGrams;
        product.HeightMm = request.HeightMm;
        product.DiameterMm = request.DiameterMm;
        product.ThicknessMm = request.ThicknessMm;
        product.JewelCount = request.JewelCount;
        product.CaseMaterial = request.CaseMaterial.Trim();
        product.StrapMaterial = request.StrapMaterial.Trim();
        product.ImageUrl = request.ImageUrl.Trim();

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
            && !await dbContext.WatchProducts.AnyAsync(
                other => other.Id != id && other.ImageUrl == previousImageUrl,
                cancellationToken))
        {
            await imageStorage.DeleteIfManagedAsync(previousImageUrl, cancellationToken);
        }

        return ToResponse(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.WatchProducts
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        if (!await dbContext.WatchProducts.AnyAsync(
                other => other.Id != id && other.ImageUrl == product.ImageUrl,
                cancellationToken))
        {
            await imageStorage.DeleteIfManagedAsync(product.ImageUrl, cancellationToken);
        }

        dbContext.WatchProducts.Remove(product);
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
        var product = await dbContext.WatchProducts
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

        if (!await dbContext.WatchProducts.AnyAsync(
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
            product.Name,
            product.Model,
            product.ShortDescription,
            product.FullDescription,
            product.Price,
            "PHP",
            product.CaliberAndMovement,
            product.Chronograph,
            product.WeightGrams,
            product.HeightMm,
            product.DiameterMm,
            product.ThicknessMm,
            product.JewelCount,
            product.CaseMaterial,
            product.StrapMaterial,
            product.ImageUrl);
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
