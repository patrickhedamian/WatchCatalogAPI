using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WatchCatalogAPI.Services;

public sealed class AzureBlobWatchImageStorage(IConfiguration configuration) : IWatchImageStorage
{
    public async Task<string> UploadAsync(
        Stream image,
        string contentType,
        CancellationToken cancellationToken)
    {
        var extension = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException("Unsupported image content type.", nameof(contentType))
        };
        var container = GetBlobServiceClient()
            .GetBlobContainerClient(GetContainerName());

        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blob = container.GetBlobClient($"products/{Guid.NewGuid():N}{extension}");
        await blob.UploadAsync(
            image,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            },
            cancellationToken);
        await container.SetAccessPolicyAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        return blob.Uri.ToString();
    }

    public async Task DeleteIfManagedAsync(string imageUrl, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri))
        {
            return;
        }

        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (imageUri.Host.EndsWith(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Azure Blob Storage is not configured. Set AzureBlobStorage__ConnectionString.");
            }

            return;
        }

        var containerName = GetContainerName();
        var pathPrefix = $"/{containerName}/";
        if (!imageUri.AbsolutePath.StartsWith(pathPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var blobName = Uri.UnescapeDataString(imageUri.AbsolutePath[pathPrefix.Length..]);
        if (!blobName.StartsWith("products/", StringComparison.Ordinal))
        {
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(blobName);
        if (!Guid.TryParseExact(fileName, "N", out _))
        {
            return;
        }

        var container = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);
        var expectedUri = container.GetBlobClient(blobName).Uri;
        if (!string.Equals(expectedUri.Authority, imageUri.Authority, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await container.GetBlobClient(blobName).DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
    }

    private BlobServiceClient GetBlobServiceClient()
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage is not configured. Set AzureBlobStorage__ConnectionString.");
        }

        return new BlobServiceClient(connectionString);
    }

    private string GetContainerName()
    {
        var containerName = configuration["AzureBlobStorage:ContainerName"];
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException(
                "The Azure Blob Storage container is not configured. Set AzureBlobStorage__ContainerName.");
        }

        return containerName;
    }
}
