using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.Features.Products.Commands.DeleteProduct
{
    public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>;

}
