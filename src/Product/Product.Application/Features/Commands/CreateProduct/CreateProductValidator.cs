using FluentValidation;

namespace ProductService.Application.Features.Commands.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Dto.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Dto.Slug).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Dto.Price).GreaterThan(0);
        RuleFor(x => x.Dto.Currency).Length(3);
    }
}