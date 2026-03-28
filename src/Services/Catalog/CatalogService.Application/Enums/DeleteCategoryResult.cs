using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Enums
{
    public enum DeleteCategoryResult
    {
        Success = 0,
        NotFound = 1,
        HasSubcategories = 2,
        HasProducts = 3
    }
}
