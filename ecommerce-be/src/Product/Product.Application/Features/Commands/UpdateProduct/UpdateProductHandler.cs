using MediatR;
using MongoDB.Driver;
using ProductService.Application.Abstractions.Persistence;
using ProductService.Application.Features.Commands.UpdateProduct;
using ProductService.Domain.Entities;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductHandler(IProductRepository repo)
    : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand c, CancellationToken ct)
    {
        var dto = c.Dto;
        var prod = new Product
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
            await repo.UpdateAsync(prod, ct);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException("Sku or Slug already exists");
        }

        return new UpdateProductResult(prod.Id, prod.Sku, prod.Name, prod.Price);
    }
}
