using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _col;

    public ProductRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<Product>("products");
    }

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _col.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _col.AsQueryable().OrderByDescending(x => x.CreatedAtUtc).ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return await GetAllAsync();

        var re = new BsonRegularExpression(keyword.Trim(), "i");
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(x => x.Name, re),
            Builders<Product>.Filter.Regex(x => x.Sku, re),
            Builders<Product>.Filter.Regex(x => x.Slug, re)
        );
        return await _col.Find(filter).SortByDescending(x => x.CreatedAtUtc).ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
        product.CreatedAtUtc = DateTime.UtcNow;
        await _col.InsertOneAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedAtUtc = DateTime.UtcNow;
        var res = await _col.ReplaceOneAsync(x => x.Id == product.Id, product);
        if (res.MatchedCount == 0) throw new KeyNotFoundException("Product not found");
    }

    public async Task DeleteAsync(Guid id)
    {
        var res = await _col.DeleteOneAsync(x => x.Id == id);
        if (res.DeletedCount == 0) throw new KeyNotFoundException("Product not found");
    }

    public async Task<(IEnumerable<Product> items, long total)> QueryAsync(
        string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        int page = 1, int pageSize = 20)
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
        if (categoryId.HasValue) filters.Add(Builders<Product>.Filter.Eq(x => x.CategoryId, categoryId.Value));
        if (minPrice.HasValue) filters.Add(Builders<Product>.Filter.Gte(x => x.Price, minPrice.Value));
        if (maxPrice.HasValue) filters.Add(Builders<Product>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count == 0 ? Builders<Product>.Filter.Empty : Builders<Product>.Filter.And(filters);

        var total = await _col.CountDocumentsAsync(filter);
        var items = await _col.Find(filter)
                              .SortByDescending(x => x.CreatedAtUtc)
                              .Skip((page - 1) * pageSize)
                              .Limit(pageSize)
                              .ToListAsync();
        return (items, total);
    }
}
