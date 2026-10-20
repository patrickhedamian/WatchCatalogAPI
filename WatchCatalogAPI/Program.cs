using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using WatchCatalogAPI.Data;
using WatchCatalogAPI.Middleware;
using WatchCatalogAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUrl = new Uri(
    "https://kv-react-demo.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(
    keyVaultUrl,
    new DefaultAzureCredential());

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<WatchCatalogDbContext>(options =>
{
    var connectionString = builder.Configuration["SqlConnectionString"];
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "The WatchCatalog SQL connection string is missing. Set ConnectionStrings__WatchCatalog.");
    }

    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IWatchProductService, WatchProductService>();
builder.Services.AddSingleton<IWatchImageStorage, AzureBlobWatchImageStorage>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(settings =>
    {
        settings.Path = "/swagger";
        settings.DocumentTitle = "Watch Catalog API";
        settings.DocExpansion = "list";
        settings.SwaggerRoutes.Add(
            new NSwag.AspNetCore.SwaggerUiRoute("Watch Catalog API", "/openapi/v1.json"));
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
