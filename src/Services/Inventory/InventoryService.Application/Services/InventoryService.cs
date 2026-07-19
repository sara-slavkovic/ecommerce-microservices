using FluentValidation;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Validators;
using InventoryService.Domain.Entities;
using SharedKernel.Domain.Exceptions;
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
                throw new ConflictException("Inventory item for this product already exists.");
            }

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                AvailableQuantity = dto.AvailableQuantity,
                ReservedQuantity = 0,
                LastUpdatedAt = DateTime.Now
            };

            await _inventoryRepository.AddInventoryItemAsync(inventoryItem);
            await _inventoryRepository.SaveChangesAsync();

            return MapToDto(inventoryItem);
        }

        public async Task DeleteInventoryItemAsync(Guid id)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(id);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            _inventoryRepository.DeleteInventoryItem(inventoryItem);
            await _inventoryRepository.SaveChangesAsync();
        }

        public async Task DeleteInventoryItemByProductIdAsync(Guid productId)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            _inventoryRepository.DeleteInventoryItem(inventoryItem);
            await _inventoryRepository.SaveChangesAsync();
        }

        public async Task<InventoryItemDto> ReserveInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            if (inventoryItem.AvailableQuantity < dto.Quantity)
            {
                throw new ConflictException("Not enough available stock.");
            }

            inventoryItem.AvailableQuantity -= dto.Quantity;
            inventoryItem.ReservedQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.SaveChangesAsync();

            return MapToDto(inventoryItem);
        }

        //when order or payment fails or order gets cancelled
        public async Task<InventoryItemDto> ReleaseInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            if (dto.Quantity > inventoryItem.ReservedQuantity)
            {
                throw new ConflictException("Not enough reserved stock to release.");
            }

            inventoryItem.ReservedQuantity -= dto.Quantity;
            inventoryItem.AvailableQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.SaveChangesAsync();

            return MapToDto(inventoryItem);
        }

        public async Task<InventoryItemDto> ConfirmDeductionAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            if (inventoryItem.ReservedQuantity < dto.Quantity)
            {
                throw new ConflictException("Not enough reserved stock to confirm deduction.");
            }

            inventoryItem.ReservedQuantity -= dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.SaveChangesAsync();

            return MapToDto(inventoryItem);
        }

        public async Task<InventoryItemDto> RestockInventoryAsync(ChangeInventoryQuantityDto dto)
        {
            var validationResult = await _quantityChangeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var inventoryItem = await _inventoryRepository.GetInventoryItemByProductIdAsync(dto.ProductId);
            if (inventoryItem == null)
            {
                throw new NotFoundException("Inventory item not found.");
            }

            inventoryItem.AvailableQuantity += dto.Quantity;
            inventoryItem.LastUpdatedAt = DateTime.Now;

            await _inventoryRepository.SaveChangesAsync();

            return MapToDto(inventoryItem);
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