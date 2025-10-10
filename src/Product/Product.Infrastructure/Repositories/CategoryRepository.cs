using MongoDB.Driver;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _collection;

    public CategoryRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Category>("categories");
    }

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _collection.Find(FilterDefinition<Category>.Empty).ToListAsync();

    public async Task AddAsync(Category category) =>
        await _collection.InsertOneAsync(category);

    public async Task UpdateAsync(Category category) =>
        await _collection.ReplaceOneAsync(x => x.Id == category.Id, category);

    public async Task DeleteAsync(Guid id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}
