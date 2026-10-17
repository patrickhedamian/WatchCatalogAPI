namespace WatchCatalogAPI.Services;

public interface IWatchImageStorage
{
    Task<string> UploadAsync(
        Stream image,
        string contentType,
        CancellationToken cancellationToken);

    Task DeleteIfManagedAsync(string imageUrl, CancellationToken cancellationToken);
}
