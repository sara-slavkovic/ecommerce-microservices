using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId);
        Task InsertCartWithItemAsync(Cart cart, CartItem item);
        Task InsertCartItemAsync(Cart cart, CartItem item);
        Task UpdateCartAndItemAsync(Cart cart, CartItem item);
        Task DeleteCartItemAndUpdateCartAsync(Guid cartItemId, Cart cart);
        Task DeleteCartAsync(Guid cartId);
    }
}
