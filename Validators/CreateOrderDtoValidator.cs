using FluentValidation;
using OrderManagementApi.Models.DTOs;

namespace OrderManagementApi.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Ürün ID'si zorunludur ve 0'dan büyük olmalıdır.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Sipariş adedi en az 1 olmalıdır.");
    }
}
