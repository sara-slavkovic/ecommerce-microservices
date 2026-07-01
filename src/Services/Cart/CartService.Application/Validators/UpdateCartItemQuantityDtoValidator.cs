using CartService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Validators
{
    public class UpdateCartItemQuantityDtoValidator : AbstractValidator<UpdateCartItemQuantityDto>
    {
        public UpdateCartItemQuantityDtoValidator()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("UserId is required.");

            RuleFor(x => x.ProductId).NotEqual(Guid.Empty).WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
