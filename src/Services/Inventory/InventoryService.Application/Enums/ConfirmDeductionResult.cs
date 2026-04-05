using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Enums
{
    public enum ConfirmDeductionResult
    {
        Success,
        InventoryItemNotFound,
        InvalidQuantity,
        InsufficientReservedQuantity
    }
}
