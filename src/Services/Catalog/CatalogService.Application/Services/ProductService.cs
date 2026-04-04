using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(IProductRepository productRepository, IInventoryServiceClient inventoryServiceClient, IValidator<CreateProductDto> createValidator, IValidator<UpdateProductDto> updateValidator)
        {
            _productRepository = productRepository;
            _inventoryServiceClient = inventoryServiceClient;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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

            var categoryExists = await _productRepository.CategoryExistsAsync(dto.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException("Category does not exist.");
            }

            var skuExists = await _productRepository.ExistsBySkuAsync(dto.Sku);
            if (skuExists)
            {
                throw new InvalidOperationException("Product with this SKU already exists.");
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
                CategoryId = dto.CategoryId
            };

            var createdProduct = await _productRepository.AddProductAsync(product);

            try
            {
                await _inventoryServiceClient.CreateInventoryItemAsync(createdProduct.Id, dto.InitialStockQuantity);
            }
            catch (Exception)
            {
                await _productRepository.DeleteProductAsync(createdProduct.Id);
                throw new InvalidOperationException("Product was created, but inventory creation failed. Product creation has been rolled back.");
            }

            return MapToDto(createdProduct);
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

            var categoryExists = await _productRepository.CategoryExistsAsync(dto.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException("Category does not exist.");
            }

            var skuExists = await _productRepository.ExistsBySkuExcludingIdAsync(dto.Sku, id);
            if (skuExists)
            {
                throw new InvalidOperationException("Another product with this SKU already exists.");
            }

            existingProduct.Sku = dto.Sku;
            existingProduct.Name = dto.Name;
            existingProduct.Brand = dto.Brand;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.CategoryId = dto.CategoryId;

            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);

            return updatedProduct == null ? null : MapToDto(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            await _inventoryServiceClient.DeleteInventoryItemByProductIdAsync(id);
            await _productRepository.DeleteProductAsync(id);
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
