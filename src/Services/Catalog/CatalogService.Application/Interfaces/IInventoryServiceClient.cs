using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Interfaces
{
    public interface IInventoryServiceClient
    {
        Task CreateInventoryItemAsync(Guid productId, int availableQuantity);
        Task DeleteInventoryItemByProductIdAsync(Guid productId);
    }
}
