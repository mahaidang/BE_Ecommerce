using MediatR;

namespace Product.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(UpdateProductDto Dto) : IRequest<UpdateProductResult>;
