using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using FluentValidation;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateCategoryDto> _createValidator;
        private readonly IValidator<UpdateCategoryDto> _updateValidator;

        public CategoryService(ICategoryRepository categoryRepository, IValidator<CreateCategoryDto> createValidator, IValidator<UpdateCategoryDto> updateValidator)
        {
            _categoryRepository = categoryRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            return categories.Select(c => MapToDto(c));
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            return category == null ? null : MapToDto(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId
            };

            await _categoryRepository.AddCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);

            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = dto.Name;
            existingCategory.ParentCategoryId = dto.ParentCategoryId;

            await _categoryRepository.SaveChangesAsync();

            return MapToDto(existingCategory);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {id} was not found.");
            }

            var hasSubcategories = await _categoryRepository.HasSubcategoriesAsync(id);
            if (hasSubcategories)
            {
                throw new ConflictException("Category has subcategories and cannot be deleted.");
            }

            var hasProducts = await _categoryRepository.HasProductsAsync(id);
            if (hasProducts)
            {
                throw new ConflictException("Category has products and cannot be deleted.");
            }

            _categoryRepository.DeleteCategory(category);
            await _categoryRepository.SaveChangesAsync();
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };
        }
    }
}
