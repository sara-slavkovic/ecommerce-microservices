using FluentValidation;
using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Validators
{
    public class UpdateInventoryItemDtoValidator : AbstractValidator<UpdateInventoryItemDto>
    {
        public UpdateInventoryItemDtoValidator()
        {
            RuleFor(x => x.AvailableQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Available quantity must be greater than or equal to 0.");

            RuleFor(x => x.ReservedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Reserved quantity must be greater than or equal to 0.");

            RuleFor(x => x)
                .Must(x => x.ReservedQuantity <= x.AvailableQuantity)
                .WithMessage("Reserved quantity cannot be greater than available quantity.");
        }
    }
}
