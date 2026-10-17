using Microsoft.AspNetCore.Http;
using WatchCatalogAPI.Contracts;

namespace WatchCatalogAPI.Services;

public interface IWatchProductService
{
    Task<IReadOnlyList<WatchProductResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<WatchProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<WatchProductResponse> CreateAsync(CreateWatchProductRequest request, CancellationToken cancellationToken);
    Task<WatchProductResponse?> UpdateAsync(int id, UpdateWatchProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<WatchProductResponse?> ReplaceImageAsync(
        int id,
        IFormFile image,
        string contentType,
        CancellationToken cancellationToken);
}
