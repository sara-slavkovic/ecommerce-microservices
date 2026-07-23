using CartService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<UserSnapshotDto?> GetUserSnapshotByIdAsync(Guid userId);
    }
}
