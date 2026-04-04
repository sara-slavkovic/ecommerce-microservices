using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryItemDto>> GetAllInventoryItemsAsync()
        {
            var inventoryItems = await _inventoryRepository.GetAllInventoryItemsAsync();

            return inventoryItems.Select(MapToDto).ToList();
        }

        public async Task<InventoryItemDto?> GetInventoryItemByIdAsync(Guid id)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(id);

            return inventoryItem == null ? null : MapToDto(inventoryItem);
        }

        public async Task<InventoryItemDto?> GetInventoryItemByProductIdAsync(Guid productId)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(productId);

            return inventoryItem == null ? null : MapToDto(inventoryItem);
        }

        public async Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemDto dto)
        {
            var exists = await _inventoryRepository.ExistsByProductIdAsync(dto.ProductId);
            if (exists)
            {
                throw new Exception("Inventory item for this product already exists.");
            }

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                AvailableQuantity = dto.AvailableQuantity,
                ReservedQuantity = 0,
                LastUpdatedAt = DateTime.UtcNow
            };

            var createdInventoryItem = await _inventoryRepository.AddInventoryItemAsync(inventoryItem);

            return MapToDto(createdInventoryItem);
        }

        public async Task<InventoryItemDto?> UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto)
        {
            var existingInventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(id);
            if (existingInventoryItem == null)
            {
                return null;
            }

            existingInventoryItem.AvailableQuantity = dto.AvailableQuantity;
            existingInventoryItem.ReservedQuantity = dto.ReservedQuantity;
            existingInventoryItem.LastUpdatedAt = DateTime.UtcNow;

            var updatedInventoryItem = await _inventoryRepository.UpdateInventoryItemAsync(existingInventoryItem);

            return updatedInventoryItem == null ? null : MapToDto(updatedInventoryItem);
        }

        public async Task<bool> DeleteInventoryItemAsync(Guid id)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(id);
            if (inventoryItem == null)
            {
                return false;
            }

            await _inventoryRepository.DeleteInventoryItemAsync(id);
            return true;
        }

        public async Task<bool> DeleteInventoryItemByProductIdAsync(Guid productId)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                return false;
            }

            await _inventoryRepository.DeleteInventoryItemByProductIdAsync(inventoryItem.ProductId);
            return true;
        }

        private static InventoryItemDto MapToDto(InventoryItem inventoryItem)
        {
            return new InventoryItemDto
            {
                Id = inventoryItem.Id,
                ProductId = inventoryItem.ProductId,
                AvailableQuantity = inventoryItem.AvailableQuantity,
                ReservedQuantity = inventoryItem.ReservedQuantity,
                LastUpdatedAt = inventoryItem.LastUpdatedAt
            };
        }
    }
}