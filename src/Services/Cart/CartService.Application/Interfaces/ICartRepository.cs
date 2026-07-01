using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId);
        Task InsertCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task<Guid> InsertCartItemAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task DeleteCartItemAsync(Guid cartItemId);
        Task DeleteCartAsync(Guid cartId);
    }
}
