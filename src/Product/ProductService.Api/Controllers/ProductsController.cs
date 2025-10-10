// Api/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;

    public ProductsController(IProductRepository repo) => _repo = repo;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        var p = new Product
        {
            Sku = dto.Sku.Trim(),
            Name = dto.Name.Trim(),
            Slug = dto.Slug.Trim().ToLowerInvariant(),
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            Currency = dto.Currency.Trim().ToUpperInvariant(),
            IsActive = dto.IsActive
        };

        try
        {
            await _repo.AddAsync(p);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Sku or Slug already exists" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? q,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize is <= 0 or > 200 ? 20 : pageSize;

        var (items, total) = await _repo.QueryAsync(q, categoryId, minPrice, maxPrice, page, pageSize);
        return Ok(new { total, page, pageSize, items });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ProductUpdateDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Sku = dto.Sku.Trim();
        existing.Name = dto.Name.Trim();
        existing.Slug = dto.Slug.Trim().ToLowerInvariant();
        existing.CategoryId = dto.CategoryId;
        existing.Price = dto.Price;
        existing.Currency = dto.Currency.Trim().ToUpperInvariant();
        existing.IsActive = dto.IsActive;

        try
        {
            await _repo.UpdateAsync(existing);
            return Ok(existing);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Sku or Slug already exists" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
