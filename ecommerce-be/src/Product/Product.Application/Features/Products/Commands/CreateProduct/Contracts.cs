using MediatR;

namespace Product.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Dto)
    : IRequest<CreateProductResult>;