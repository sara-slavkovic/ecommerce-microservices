using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItem>> GetAllInventoryItemsAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        public async Task<InventoryItem?> GetInventoryItemByIdAsync(Guid id)
        {
            return await _context.InventoryItems.FindAsync(id);
        }

        public async Task<InventoryItem?> GetInventoryItemByProductIdAsync(Guid productId)
        {
            return await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<bool> ExistsByProductIdAsync(Guid productId)
        {
            return await _context.InventoryItems.AnyAsync(i => i.ProductId == productId);
        }

        public async Task<InventoryItem> AddInventoryItemAsync(InventoryItem inventoryItem)
        {
            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();
            return inventoryItem;
        }

        public async Task<InventoryItem?> UpdateInventoryItemAsync(InventoryItem inventoryItem)
        {
            var existingInventoryItem = await _context.InventoryItems.FindAsync(inventoryItem.Id);

            if (existingInventoryItem == null)
            {
                return null;
            }

            existingInventoryItem.AvailableQuantity = inventoryItem.AvailableQuantity;
            existingInventoryItem.ReservedQuantity = inventoryItem.ReservedQuantity;
            existingInventoryItem.LastUpdatedAt = inventoryItem.LastUpdatedAt;

            await _context.SaveChangesAsync();
            return existingInventoryItem;
        }

        public async Task DeleteInventoryItemAsync(Guid id)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(id);

            if (inventoryItem == null)
            {
                return;
            }

            _context.InventoryItems.Remove(inventoryItem);
            await _context.SaveChangesAsync();
        }
    }
}