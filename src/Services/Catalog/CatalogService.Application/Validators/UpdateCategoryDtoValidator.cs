using CatalogService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Validators
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.ParentCategoryId)
                .NotEqual(Guid.Empty).When(x => x.ParentCategoryId.HasValue)
                .WithMessage("ParentCategoryId cannot be an empty GUID.");
        }
    }
}
