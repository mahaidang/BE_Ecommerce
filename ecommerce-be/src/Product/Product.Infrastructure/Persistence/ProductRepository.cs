using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Product.Application.Abstractions.Persistence;
using Product.Domain.Entities;

namespace Product.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductModel> _col;

    public ProductRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<ProductModel>("products");
    }

    public async Task<ProductModel?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<ProductModel>> GetAllAsync(CancellationToken ct) =>
        await _col.AsQueryable().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public async Task<IEnumerable<ProductModel>> SearchAsync(string? keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return await GetAllAsync(ct);

        var re = new BsonRegularExpression(keyword.Trim(), "i");
        var filter = Builders<ProductModel>.Filter.Or(
            Builders<ProductModel>.Filter.Regex(x => x.Name, re),
            Builders<ProductModel>.Filter.Regex(x => x.Sku, re),
            Builders<ProductModel>.Filter.Regex(x => x.Slug, re)
        );

        return await _col.Find(filter)
                         .SortByDescending(x => x.CreatedAtUtc)
                         .ToListAsync(ct);
    }

    public async Task AddAsync(ProductModel product, CancellationToken ct)
    {
        if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
        product.CreatedAtUtc = DateTime.UtcNow;
        await _col.InsertOneAsync(product, cancellationToken: ct);
    }

    public async Task UpdateAsync(ProductModel product, CancellationToken ct)
    {
        var update = Builders<ProductModel>.Update
             .Set(x => x.Sku, product.Sku)
             .Set(x => x.Name, product.Name)
             .Set(x => x.Slug, product.Slug)
             .Set(x => x.CategoryId, product.CategoryId)
             .Set(x => x.Price, product.Price)
             .Set(x => x.Currency, product.Currency)
             .Set(x => x.IsActive, product.IsActive)
             .Set(x => x.UpdatedAtUtc, DateTime.UtcNow);

        var res = await _col.UpdateOneAsync(
            x => x.Id == product.Id,
            update,
            cancellationToken: ct
        );

        if (res.MatchedCount == 0)
            throw new KeyNotFoundException("Product not found");
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var res = await _col.DeleteOneAsync(x => x.Id == id, ct);
        if (res.DeletedCount == 0) throw new KeyNotFoundException("Product not found");
    }

    public async Task<(IEnumerable<ProductModel> items, long total)> QueryAsync(
        string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct)
    {
        var filters = new List<FilterDefinition<ProductModel>>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var re = new BsonRegularExpression(keyword.Trim(), "i");
            filters.Add(Builders<ProductModel>.Filter.Or(
                Builders<ProductModel>.Filter.Regex(x => x.Name, re),
                Builders<ProductModel>.Filter.Regex(x => x.Sku, re),
                Builders<ProductModel>.Filter.Regex(x => x.Slug, re)
            ));
        }
        if (categoryId.HasValue)
            filters.Add(Builders<ProductModel>.Filter.Eq(x => x.CategoryId, categoryId.Value));
        if (minPrice.HasValue)
            filters.Add(Builders<ProductModel>.Filter.Gte(x => x.Price, minPrice.Value));
        if (maxPrice.HasValue)
            filters.Add(Builders<ProductModel>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count == 0
            ? Builders<ProductModel>.Filter.Empty
            : Builders<ProductModel>.Filter.And(filters);

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
        var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, id);
        return await _col.Find(filter).AnyAsync(ct);
    }
}
