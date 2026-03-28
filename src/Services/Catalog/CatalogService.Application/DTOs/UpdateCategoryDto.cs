using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.DTOs
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }
}