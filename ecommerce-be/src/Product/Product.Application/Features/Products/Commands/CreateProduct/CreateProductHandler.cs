using MediatR;
using MongoDB.Driver;
using Product.Application.Abstractions.Persistence;
using Product.Domain.Entities;


namespace Product.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _prods;

    public CreateProductHandler(IProductRepository prods)
    {
        _prods = prods;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand req, CancellationToken ct)
    {
        var dto = req.Dto;
        var prod = new ProductModel
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
            await _prods.AddAsync(prod, ct);
        }
        catch (MongoWriteException mwe) when (mwe.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException("Sku or Slug already exists");
        }

        return new CreateProductResult(prod.Id, prod.Sku, prod.Name, prod.Price);
    }
}
