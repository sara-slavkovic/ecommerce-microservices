using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _context;

        public ProductRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductBySkuAsync(string sku)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Sku == sku);
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);

            if(existingProduct == null)
            {
                return null;
            }   

            existingProduct.Sku = product.Sku;
            existingProduct.Name = product.Name;
            existingProduct.Brand = product.Brand;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.IsActive = product.IsActive;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsBySkuAsync(string sku)
        {
            return await _context.Products.AnyAsync(p => p.Sku == sku);
        }

        public async Task<bool> ExistsBySkuExcludingIdAsync(string sku, Guid excludeId)
        {
            return await _context.Products.AnyAsync(p => p.Sku == sku && p.Id != excludeId);    
        }
    }
}
