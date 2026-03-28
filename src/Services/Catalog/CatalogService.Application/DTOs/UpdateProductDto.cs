using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.DTOs
{
    public class UpdateProductDto
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
    }
}
