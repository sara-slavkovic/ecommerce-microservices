using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.DTOs
{
    public class CreateInventoryItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
