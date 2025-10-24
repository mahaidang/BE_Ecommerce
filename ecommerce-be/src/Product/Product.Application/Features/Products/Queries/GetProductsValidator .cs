using FluentValidation;

namespace Product.Application.Features.Products.Queries;

public sealed class GetProductsValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x).Must(x => !(x.MinPrice.HasValue && x.MaxPrice.HasValue) || x.MinPrice <= x.MaxPrice)
            .WithMessage("minPrice must be <= maxPrice");
    }
}
