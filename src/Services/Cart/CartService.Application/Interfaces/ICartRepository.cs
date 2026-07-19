using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId);
        Task AddCartAsync(Cart cart);
        void DeleteCart(Cart cart);
        Task SaveChangesAsync();
    }
}
