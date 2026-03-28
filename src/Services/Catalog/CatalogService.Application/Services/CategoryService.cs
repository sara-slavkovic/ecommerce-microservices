using CatalogService.Application.DTOs;
using CatalogService.Application.Enums;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            return category == null ? null : new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId
            };

            var createdCategory = await _categoryRepository.AddCategoryAsync(category);

            return new CategoryDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                ParentCategoryId = createdCategory.ParentCategoryId
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);

            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = dto.Name;
            existingCategory.ParentCategoryId = dto.ParentCategoryId;

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(existingCategory);

            return updatedCategory == null ? null : new CategoryDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name,
                ParentCategoryId = updatedCategory.ParentCategoryId
            };
        }

        public async Task<DeleteCategoryResult> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return DeleteCategoryResult.NotFound;
            }

            var hasSubcategories = await _categoryRepository.HasSubcategoriesAsync(id);
            if (hasSubcategories)
            {
                return DeleteCategoryResult.HasSubcategories;
            }

            var hasProducts = await _categoryRepository.HasProductsAsync(id);
            if (hasProducts)
            {
                return DeleteCategoryResult.HasProducts;
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return DeleteCategoryResult.Success;
        }
    }
}
