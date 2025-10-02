using FluentValidation;
using ECommerce.Shared.Dtos.Sales.Request;

namespace ECommerce.Application.Validators.SaleValidators;

public class UpdateSaleStatusRequestValidator : AbstractValidator<UpdateSaleStatusRequest>
{
    public UpdateSaleStatusRequestValidator()
    {
        RuleFor(x => x.UpdatedStatus)
            .IsInEnum().WithMessage("Invalid sale status");
    }
}