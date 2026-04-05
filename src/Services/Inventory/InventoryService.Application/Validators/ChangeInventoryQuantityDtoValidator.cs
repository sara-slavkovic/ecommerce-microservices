using FluentValidation;
using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Validators
{
    public class ChangeInventoryQuantityDtoValidator : AbstractValidator<ChangeInventoryQuantityDto>
    {
        public ChangeInventoryQuantityDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEqual(Guid.Empty).WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
