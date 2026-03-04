using ErpDemo.Application.DTOs;
using FluentValidation;

namespace ErpDemo.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Cliente é obrigatório");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Pedido deve ter pelo menos um item");

        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemValidator());
    }
}

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição do item é obrigatória")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Preço unitário deve ser maior que zero");
    }
}
