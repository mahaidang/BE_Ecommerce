using MediatR;

namespace Product.Application.Features.Commands.UpdateProduct;

public sealed record UpdateProductCommand(UpdateProductDto Dto) : IRequest<UpdateProductResult>;
