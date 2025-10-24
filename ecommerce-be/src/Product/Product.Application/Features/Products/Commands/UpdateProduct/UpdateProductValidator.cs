using FluentValidation;

namespace Product.Application.Features.Products.Commands.UpdateProduct;
public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Dto.Id).NotEmpty();
        RuleFor(x => x.Dto.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Dto.Slug).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Dto.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dto.Currency).Length(3);
    }
}
