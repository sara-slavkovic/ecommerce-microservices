using InventoryService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<InventoryItem>> GetAllInventoryItemsAsync();
        Task<InventoryItem?> GetInventoryItemByIdAsync(Guid id);
        Task<InventoryItem?> GetInventoryItemByProductIdAsync(Guid productId);
        Task<bool> ExistsByProductIdAsync(Guid productId);
        Task<InventoryItem> AddInventoryItemAsync(InventoryItem inventoryItem);
        Task<InventoryItem?> UpdateInventoryItemAsync(InventoryItem inventoryItem);
        Task DeleteInventoryItemAsync(Guid id);
    }
}
