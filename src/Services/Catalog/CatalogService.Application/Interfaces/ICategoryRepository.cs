using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(Category category);
        void DeleteCategory(Category category);
        Task<bool> HasSubcategoriesAsync(Guid categoryId);
        Task<bool> HasProductsAsync(Guid categoryId);
        Task SaveChangesAsync();
    }
}
