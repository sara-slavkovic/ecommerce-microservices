using CartService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartByUserIdAsync(Guid userId);
        Task<CartDto> AddItemToCartAsync(CreateCartItemDto dto);
        Task<CartDto?> UpdateCartItemQuantityAsync(UpdateCartItemQuantityDto dto);
        Task<bool> RemoveCartItemAsync(Guid userId, Guid productId);
        Task<bool> DeleteCartAsync(Guid userId);
    }
}
