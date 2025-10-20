using MediatR;
using ProductService.Application.Features.Commands.UpdateProduct;

namespace ProductService.Application.Features.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Dto)
    : IRequest<CreateProductResult>;