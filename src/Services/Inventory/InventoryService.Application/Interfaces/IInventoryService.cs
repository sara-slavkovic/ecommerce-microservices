using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItemDto>> GetAllInventoryItemsAsync();
        Task<InventoryItemDto?> GetInventoryItemByIdAsync(Guid id);
        Task<InventoryItemDto?> GetInventoryItemByProductIdAsync(Guid productId);
        Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemDto dto);
        Task<InventoryItemDto?> UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto);
        Task<bool> DeleteInventoryItemAsync(Guid id);
        Task<bool> DeleteInventoryItemByProductIdAsync(Guid productId);
    }
}