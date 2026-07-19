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
        Task DeleteInventoryItemAsync(Guid id);
        Task DeleteInventoryItemByProductIdAsync(Guid productId);
        Task<InventoryItemDto> ReserveInventoryAsync(ChangeInventoryQuantityDto dto);
        Task<InventoryItemDto> ReleaseInventoryAsync(ChangeInventoryQuantityDto dto);
        Task<InventoryItemDto> ConfirmDeductionAsync(ChangeInventoryQuantityDto dto);
        Task<InventoryItemDto> RestockInventoryAsync(ChangeInventoryQuantityDto dto);
    }
}