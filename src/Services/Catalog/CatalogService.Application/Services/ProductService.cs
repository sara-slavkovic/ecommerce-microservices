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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IInventoryServiceClient inventoryServiceClient, IValidator<CreateProductDto> createValidator, IValidator<UpdateProductDto> updateValidator)
        {
            _productRepository = productRepository;
            _inventoryServiceClient = inventoryServiceClient;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();

            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);

            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new NotFoundException("Category does not exist.");
            }

            var skuExists = await _productRepository.ExistsBySkuAsync(dto.Sku);
            if (skuExists)
            {
                throw new ConflictException("Product with this SKU already exists.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Sku = dto.Sku,
                Name = dto.Name,
                Brand = dto.Brand,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CategoryId = dto.CategoryId,
                Category = category
            };

            await _productRepository.AddProductAsync(product);

            try
            {
                await _inventoryServiceClient.CreateInventoryItemAsync(product.Id, dto.InitialStockQuantity);
            }
            catch (Exception)
            {
                throw new ConflictException("Failed to create product inventory tracking. Operation aborted.");
            }

            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new NotFoundException("Category does not exist.");
            }

            var skuExists = await _productRepository.ExistsBySkuExcludingIdAsync(dto.Sku, id);
            if (skuExists)
            {
                throw new ConflictException("Another product with this SKU already exists.");
            }

            existingProduct.Sku = dto.Sku;
            existingProduct.Name = dto.Name;
            existingProduct.Brand = dto.Brand;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.Category = category;

            await _productRepository.SaveChangesAsync();

            return MapToDto(existingProduct);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            _productRepository.DeleteProduct(product);
            await _productRepository.SaveChangesAsync();

            try
            {
                await _inventoryServiceClient.DeleteInventoryItemByProductIdAsync(id);
            }
            catch (Exception)
            {
                throw new ConflictException($"Product {product.Name} was deleted, but inventory cleanup failed.");
            }

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Brand = product.Brand,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : string.Empty
            };
        }
    }
}
