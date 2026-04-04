using CatalogService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is required.")
                .MaximumLength(100).WithMessage("Brand must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.");

            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty).WithMessage("CategoryId is required.");

            RuleFor(x => x.InitialStockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Initial stock must be greater than or equal to 0.");
        }
    }
}
