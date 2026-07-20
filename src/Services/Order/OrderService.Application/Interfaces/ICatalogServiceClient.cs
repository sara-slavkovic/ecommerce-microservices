using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface ICatalogServiceClient
    {
        Task<ProductSnapshotDto?> GetProductSnapshotByIdAsync(Guid productId);
    }
}
