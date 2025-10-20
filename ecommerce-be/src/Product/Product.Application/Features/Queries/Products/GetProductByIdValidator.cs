using FluentValidation;

namespace ProductService.Application.Features.Queries.Products;
public class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .NotEqual(Guid.Empty).WithMessage("Id must be a valid GUID");
    }
}

