using FluentValidation;
using InventoryService.Application.DTOs;
using InventoryService.Application.Enums;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Validators;
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
        private readonly IValidator<CreateInventoryItemDto> _createValidator;
        private readonly IValidator<ChangeInventoryQuantityDto> _quantityChangeValidator;

        public InventoryService(IInventoryRepository inventoryRepository, IValidator<CreateInventoryItemDto> createValidator, IValidator<ChangeInventoryQuantityDto> quantityChangeValidator)
        {
            _inventoryRepository = inventoryRepository;
            _createValidator = createValidator;
            _quantityChangeValidator = quantityChangeValidator;
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
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var exists = await _inventoryRepository.ExistsByProductIdAsync(dto.ProductId);
            if (exists)
            {
                throw new InvalidOperationException("Inventory item for this product already exists.");
            }

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                AvailableQuantity = dto.AvailableQuantity,
                ReservedQuantity = 0,
                LastUpdatedAt = DateTime.Now
            };

            var createdInventoryItem = await _inventoryRepository.AddInventoryItemAsync(inventoryItem);

            return MapToDto(createdInventoryItem);
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

        public async Task<ReserveInventoryResult> ReserveInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return ReserveInventoryResult.InvalidQuantity;
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                return ReserveInventoryResult.InventoryItemNotFound;
            }

            if (inventoryItem.AvailableQuantity < dto.Quantity)
            {
                return ReserveInventoryResult.InsufficientAvailableQuantity;
            }

            inventoryItem.AvailableQuantity -= dto.Quantity;
            inventoryItem.ReservedQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem);

            return ReserveInventoryResult.Success;
        }

        //when order or payment fails or order gets cancelled
        public async Task<ReleaseInventoryResult> ReleaseInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return ReleaseInventoryResult.InvalidQuantity;
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                return ReleaseInventoryResult.InventoryItemNotFound;
            }

            if (dto.Quantity > inventoryItem.ReservedQuantity)
            {
                return ReleaseInventoryResult.InsufficientReservedQuantity;
            }

            inventoryItem.ReservedQuantity -= dto.Quantity;
            inventoryItem.AvailableQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem);

            return ReleaseInventoryResult.Success;
        }

        public async Task<ConfirmDeductionResult> ConfirmDeductionAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return ConfirmDeductionResult.InvalidQuantity;
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                return ConfirmDeductionResult.InventoryItemNotFound;
            }

            if (inventoryItem.ReservedQuantity < dto.Quantity)
            {
                return ConfirmDeductionResult.InsufficientReservedQuantity;
            }

            inventoryItem.ReservedQuantity -= dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem);

            return ConfirmDeductionResult.Success;
        }

        public async Task<bool> RestockInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                return false;
            }

            inventoryItem.AvailableQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem);

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