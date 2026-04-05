using InventoryService.Application.DTOs;
using InventoryService.Application.Enums;
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
        Task<bool> DeleteInventoryItemAsync(Guid id);
        Task<bool> DeleteInventoryItemByProductIdAsync(Guid productId);
        Task<ReserveInventoryResult> ReserveInventoryAsync(ChangeInventoryQuantityDto dto);
        Task<ReleaseInventoryResult> ReleaseInventoryAsync(ChangeInventoryQuantityDto dto);
        Task<ConfirmDeductionResult> ConfirmDeductionAsync(ChangeInventoryQuantityDto dto);
        Task<bool> RestockInventoryAsync(ChangeInventoryQuantityDto dto);
    }
}