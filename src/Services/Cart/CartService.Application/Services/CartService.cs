using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using FluentValidation;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICatalogServiceClient _catalogServiceClient;
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IValidator<CreateCartItemDto> _createValidator;
        private readonly IValidator<UpdateCartItemQuantityDto> _updateValidator;

        public CartService(ICartRepository cartRepository, ICatalogServiceClient catalogServiceClient, IInventoryServiceClient inventoryServiceClient, IValidator<CreateCartItemDto> createValidator, IValidator<UpdateCartItemQuantityDto> updateValidator)
        {
            _cartRepository = cartRepository;
            _catalogServiceClient = catalogServiceClient;
            _inventoryServiceClient = inventoryServiceClient;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null) return null;
            return await MapToCartDtoAsync(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(CreateCartItemDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var (product, inventory) = await GetProductAndInventoryAsync(dto.ProductId);

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(dto.UserId);
            var isNewCart = cart == null;

            if (isNewCart)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    TotalAmount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
            }

            var existingItem = cart!.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;
                if (newQuantity > inventory.AvailableQuantity)
                    throw new ConflictException("Not enough stock available.");

                existingItem.Quantity = newQuantity;
                existingItem.PricePerUnit = product.Price;
            }
            else
            {
                if (dto.Quantity > inventory.AvailableQuantity)
                    throw new ConflictException("Not enough stock available.");

                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    PricePerUnit = product.Price
                });
            }

            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.PricePerUnit);
            cart.UpdatedAt = DateTime.Now;

            if (isNewCart)
            {
                await _cartRepository.AddCartAsync(cart);
            }

            await _cartRepository.SaveChangesAsync();

            return await MapToCartDtoAsync(cart);
        }

        public async Task<CartDto?> UpdateCartItemQuantityAsync(UpdateCartItemQuantityDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(dto.UserId);
            if (cart == null) return null;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (cartItem == null) return null;

            var (product, inventory) = await GetProductAndInventoryAsync(dto.ProductId);

            if (dto.Quantity > inventory.AvailableQuantity)
                throw new ConflictException("Not enough stock available.");

            cartItem.Quantity = dto.Quantity;
            cartItem.PricePerUnit = product.Price;

            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.PricePerUnit);
            cart.UpdatedAt = DateTime.Now;

            await _cartRepository.SaveChangesAsync();

            return await MapToCartDtoAsync(cart);
        }

        public async Task<bool> RemoveCartItemAsync(Guid userId, Guid productId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null) return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null) return false;

            cart.CartItems.Remove(cartItem);
            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.PricePerUnit);
            cart.UpdatedAt = DateTime.Now;

            await _cartRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null) return false;

            _cartRepository.DeleteCart(cart);
            await _cartRepository.SaveChangesAsync();

            return true;
        }

        private async Task<(ProductSnapshotDto product, InventoryAvailabilityDto inventory)> GetProductAndInventoryAsync(Guid productId)
        {
            var productTask = _catalogServiceClient.GetProductSnapshotByIdAsync(productId);
            var inventoryTask = _inventoryServiceClient.GetInventoryByProductIdAsync(productId);
            await Task.WhenAll(productTask, inventoryTask);

            var product = productTask.Result;
            var inventory = inventoryTask.Result;

            if (product == null)
                throw new NotFoundException("Product not found.");
            if (!product.IsActive)
                throw new BadRequestException("Product is not available for purchase.");
            if (inventory == null)
                throw new NotFoundException("Inventory not found.");

            return (product, inventory);
        }

        private async Task<CartDto> MapToCartDtoAsync(Cart cart)
        {
            var cartItemDtos = new List<CartItemDto>();

            foreach (var item in cart.CartItems)
            {
                var product = await _catalogServiceClient.GetProductSnapshotByIdAsync(item.ProductId);

                cartItemDtos.Add(new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? string.Empty,
                    ProductImageUrl = product?.ImageUrl ?? string.Empty,
                    Quantity = item.Quantity,
                    PricePerUnit = item.PricePerUnit,
                    TotalPrice = item.Quantity * item.PricePerUnit
                });
            }

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                TotalAmount = cart.TotalAmount,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                CartItems = cartItemDtos
            };
        }
    }
}
