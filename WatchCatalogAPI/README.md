# Watch Catalog API

ASP.NET Core 10 Web API for managing watch products. Product data is stored in
Azure SQL through Entity Framework Core; image uploads use Azure Blob Storage.

## Configuration

Set configuration outside source control (for example, environment variables in
Azure App Service):

| Environment variable | Description |
| --- | --- |
| `ConnectionStrings__WatchCatalog` | Azure SQL connection string |
| `AzureBlobStorage__ConnectionString` | Azure Storage connection string |
| `AzureBlobStorage__ContainerName` | Container for watch images (for example, `watch-images`) |

The blob container is created on the first upload with blob-level public read
access so the returned image URLs can be displayed by a separate React app.
Configure the storage account to permit public blob access. Database and storage
credentials are never returned by the API.

## Database migrations and seed data

The initial migration creates the `WatchProducts` table and inserts ten sample
products. After setting `ConnectionStrings__WatchCatalog`, run from this
directory:

```powershell
dotnet ef database update
```

For subsequent model changes, create and apply migrations with
`dotnet ef migrations add <MigrationName>` and `dotnet ef database update`.
Migrations are not automatically applied at application startup.

## API

When run with the Development launch profile, Swagger UI opens at
`/swagger` and displays the API endpoints from the built-in OpenAPI document
at `/openapi/v1.json`. For example, the local HTTP profile uses
`http://localhost:5196/swagger`.

The `WatchCatalogAPI.http` file demonstrates list/detail/create/update/delete
and image replacement operations. Prices are returned as decimal PHP amounts
with `currency: "PHP"`. Image replacement accepts JPEG, PNG, or WebP files up
to 10 MB. Deleting a product removes its managed Blob image only when no other
product references that same URL.

Product list and detail responses are cached in the application process using
`IMemoryCache` for five minutes. Successful create, update, delete, and image
replacement operations invalidate the list and affected product entry. This is
an in-memory per-instance cache; each running API instance has an independent
cache.
