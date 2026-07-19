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
        Task AddInventoryItemAsync(InventoryItem inventoryItem);
        void DeleteInventoryItem(InventoryItem inventoryItem);
        Task SaveChangesAsync();
    }
}
