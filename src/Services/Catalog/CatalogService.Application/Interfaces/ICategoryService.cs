using CatalogService.Application.DTOs;
using CatalogService.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task<DeleteCategoryResult> DeleteCategoryAsync(Guid id);
    }
}
