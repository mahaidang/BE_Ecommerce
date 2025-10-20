using MediatR;
using ProductService.Application.Abstractions.Persistence;

namespace ProductService.Application.Features.Commands.DeleteProduct;

public sealed class DeleteProductHandler(IProductRepository repo)
    : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand c, CancellationToken ct)
    {
        await repo.DeleteAsync(c.Id, ct); // repo throw KeyNotFound nếu không tồn tại
        return Unit.Value;
    }
}
