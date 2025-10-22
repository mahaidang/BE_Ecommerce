using MediatR;

namespace Product.Application.Features.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Dto)
    : IRequest<CreateProductResult>;