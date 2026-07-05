using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface ICartServiceClient
    {
        Task<CartSnapshotDto?> GetCartByUserIdAsync(Guid userId);
        Task DeleteCartAsync(Guid userId);
    }
}
