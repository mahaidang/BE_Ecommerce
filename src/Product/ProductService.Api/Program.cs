using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ProductService.Application.Interfaces;
using ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mongo config
var mongoCfg = builder.Configuration.GetSection("Mongo");
var connStr = mongoCfg.GetValue<string>("ConnectionString")!;
var dbName = mongoCfg.GetValue<string>("Database")!;

// Mongo DI
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connStr));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));


// Đăng ký repository
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((doc, req) =>
    {
        // Detect nếu request đến từ Gateway (qua port 5000)
        var isViaGateway = req.Host.Port == 5000 ||
                          req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                          req.Headers["Referer"].ToString().Contains(":5000");

        if (isViaGateway)
        {
            // Force URL qua Gateway
            doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5000/api/product",
                    Description = "Via Gateway"
                }
            };
        }
        else
        {
            // Chạy trực tiếp service
            doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = $"{req.Scheme}://{req.Host.Value}",
                    Description = "Direct Access"
                }
            };
        }
    });
});

app.UseHttpsRedirection();

#region crud
//// CREATE
//app.MapPost("/api/products", async (ProductCreateDto dto, IProductRepository repo) =>
//{
//    var p = new Product
//    {
//        Sku = dto.Sku.Trim(),
//        Name = dto.Name.Trim(),
//        Slug = dto.Slug.Trim().ToLowerInvariant(),
//        CategoryId = dto.CategoryId,
//        Price = dto.Price,
//        Currency = dto.Currency.Trim().ToUpperInvariant(),
//        IsActive = dto.IsActive
//    };

//    try
//    {
//        await repo.AddAsync(p);
//        return Results.Created($"/api/products/{p.Id}", p);
//    }
//    catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
//    {
//        return Results.Conflict("Sku or Slug already exists");
//    }
//});

//// READ by id
//app.MapGet("/api/products/{id:guid}", async (Guid id, IProductRepository repo) =>
//{
//    var p = await repo.GetByIdAsync(id);
//    return p is null ? Results.NotFound() : Results.Ok(p);
//});

//// LIST + filter + paging
//app.MapGet("/api/products", async (
//    string? q, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
//    int page, int pageSize, IProductRepository repo) =>
//{
//    page = page <= 0 ? 1 : page;
//    pageSize = pageSize is <= 0 or > 200 ? 20 : pageSize;

//    var (items, total) = await repo.QueryAsync(q, categoryId, minPrice, maxPrice, page, pageSize);
//    return Results.Ok(new { total, page, pageSize, items });
//});

//// UPDATE
//app.MapPut("/api/products/{id:guid}", async (Guid id, ProductUpdateDto dto, IProductRepository repo) =>
//{
//    var existing = await repo.GetByIdAsync(id);
//    if (existing is null) return Results.NotFound();

//    existing.Sku = dto.Sku.Trim();
//    existing.Name = dto.Name.Trim();
//    existing.Slug = dto.Slug.Trim().ToLowerInvariant();
//    existing.CategoryId = dto.CategoryId;
//    existing.Price = dto.Price;
//    existing.Currency = dto.Currency.Trim().ToUpperInvariant();
//    existing.IsActive = dto.IsActive;

//    try
//    {
//        await repo.UpdateAsync(existing);
//        return Results.Ok(existing);
//    }
//    catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
//    {
//        return Results.Conflict("Sku or Slug already exists");
//    }
//});

//// DELETE
//app.MapDelete("/api/products/{id:guid}", async (Guid id, IProductRepository repo) =>
//{
//    try
//    {
//        await repo.DeleteAsync(id);
//        return Results.NoContent();
//    }
//    catch (KeyNotFoundException)
//    {
//        return Results.NotFound();
//    }
//});
#endregion

app.MapControllers();


app.Run();
