using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductService.Api.Contracts.CreateProduct;
using ProductService.Api.Contracts.UpdateProduct;
using ProductService.Application.Abstractions.Persistence;
using ProductService.Application.Features.Commands.CreateProduct;
using ProductService.Application.Features.Commands.DeleteProduct;
using ProductService.Application.Features.Commands.UpdateProduct;
using ProductService.Application.Features.Queries.Products;
using System.Reflection;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly ISender _sender;
    public ProductsController(IProductRepository repo, ISender sender)
    {
        _repo = repo;
        _sender = sender;
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateRequest req, CancellationToken ct)
    {
        var dto = req.Adapt<CreateProductDto>();
        var cmd = new CreateProductCommand(dto);
        var res = await _sender.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);

    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var p = await _sender.Send(new GetProductByIdQuery(id), ct);
        return p is null ? NotFound() : Ok(p.Adapt<ProductDto>());
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? keyword,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetProductsQuery(keyword, categoryId, minPrice, maxPrice, page, pageSize), ct);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest req, CancellationToken ct)
    {
        var exists = await _repo.ExistsAsync(id, ct);
        if (!exists) return NotFound();


        var dto = req.Adapt<UpdateProductDto>() with { Id = id };

        try
        {
            var cmd = new UpdateProductCommand(dto);
            var res = await _sender.Send(cmd, ct);
            return Ok(res);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Sku or Slug already exists" });
        }
    }

    // DELETE /api/v1/products/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new DeleteProductCommand(id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
