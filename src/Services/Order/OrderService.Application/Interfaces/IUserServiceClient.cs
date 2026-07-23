using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<UserSnapshotDto?> GetUserSnapshotByIdAsync(Guid userId);
    }
}
