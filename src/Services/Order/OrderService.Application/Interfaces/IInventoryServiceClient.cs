using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface IInventoryServiceClient
    {
        Task ReserveStockAsync(InventoryChangeDto dto);
        Task ReleaseStockAsync(InventoryChangeDto dto);
        Task ConfirmStockDeductionAsync(InventoryChangeDto dto);
    }
}
