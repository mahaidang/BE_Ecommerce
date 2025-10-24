using MediatR;
using Product.Application.Abstractions.Persistence;

namespace Product.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductHandler(IProductRepository repo)
    : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand c, CancellationToken ct)
    {
        await repo.DeleteAsync(c.Id, ct); // repo throw KeyNotFound nếu không tồn tại
        return Unit.Value;
    }
}
