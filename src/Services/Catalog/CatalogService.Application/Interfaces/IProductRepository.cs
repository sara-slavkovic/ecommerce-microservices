using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Product?> GetProductBySkuAsync(string sku);
        Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
        Task<Product> AddProductAsync(Product product);
        Task<Product?> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
        Task<bool> ExistsBySkuAsync(string sku);
        Task<bool> ExistsBySkuExcludingIdAsync(string sku, Guid excludeId);
        Task<bool> CategoryExistsAsync(Guid id);
    }
}
