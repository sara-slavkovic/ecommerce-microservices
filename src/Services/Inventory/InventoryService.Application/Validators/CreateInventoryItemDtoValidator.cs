using FluentValidation;
using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Validators
{
    public class CreateInventoryItemDtoValidator : AbstractValidator<CreateInventoryItemDto>
    {
        public CreateInventoryItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEqual(Guid.Empty).WithMessage("ProductId is required.");

            RuleFor(x => x.AvailableQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Available quantity must be greater than or equal to 0.");
        }
    }
}
