using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto dto);
        Task<UserDto> LoginAsync(LoginUserDto dto);
        Task<UserSnapshotDto?> GetUserSnapshotByIdAsync(Guid id);
    }
}
