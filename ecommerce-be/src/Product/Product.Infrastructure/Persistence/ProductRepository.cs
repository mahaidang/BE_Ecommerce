using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductService.Application.Abstractions.Persistence;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _col;

    public ProductRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<Product>("products");
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct) =>
        await _col.AsQueryable().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public async Task<IEnumerable<Product>> SearchAsync(string? keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return await GetAllAsync(ct);

        var re = new BsonRegularExpression(keyword.Trim(), "i");
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(x => x.Name, re),
            Builders<Product>.Filter.Regex(x => x.Sku, re),
            Builders<Product>.Filter.Regex(x => x.Slug, re)
        );

        return await _col.Find(filter)
                         .SortByDescending(x => x.CreatedAtUtc)
                         .ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
        product.CreatedAtUtc = DateTime.UtcNow;
        await _col.InsertOneAsync(product, cancellationToken: ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        product.UpdatedAtUtc = DateTime.UtcNow;
        var res = await _col.ReplaceOneAsync(x => x.Id == product.Id, product, cancellationToken: ct);
        if (res.MatchedCount == 0) throw new KeyNotFoundException("Product not found");
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var res = await _col.DeleteOneAsync(x => x.Id == id, ct);
        if (res.DeletedCount == 0) throw new KeyNotFoundException("Product not found");
    }

    public async Task<(IEnumerable<Product> items, long total)> QueryAsync(
        string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct)
    {
        var filters = new List<FilterDefinition<Product>>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var re = new BsonRegularExpression(keyword.Trim(), "i");
            filters.Add(Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(x => x.Name, re),
                Builders<Product>.Filter.Regex(x => x.Sku, re),
                Builders<Product>.Filter.Regex(x => x.Slug, re)
            ));
        }
        if (categoryId.HasValue)
            filters.Add(Builders<Product>.Filter.Eq(x => x.CategoryId, categoryId.Value));
        if (minPrice.HasValue)
            filters.Add(Builders<Product>.Filter.Gte(x => x.Price, minPrice.Value));
        if (maxPrice.HasValue)
            filters.Add(Builders<Product>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count == 0
            ? Builders<Product>.Filter.Empty
            : Builders<Product>.Filter.And(filters);

        var total = await _col.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await _col.Find(filter)
                              .SortByDescending(x => x.CreatedAtUtc)
                              .Skip((page - 1) * pageSize)
                              .Limit(pageSize)
                              .ToListAsync(ct);

        return (items, total);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        return await _col.Find(filter).AnyAsync(ct);
    }
}
