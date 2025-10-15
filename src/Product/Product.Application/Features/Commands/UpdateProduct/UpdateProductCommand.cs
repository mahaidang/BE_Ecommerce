using MediatR;

namespace ProductService.Application.Features.Commands.UpdateProduct;

public sealed record UpdateProductCommand(UpdateProductDto Dto) : IRequest<UpdateProductResult>;
