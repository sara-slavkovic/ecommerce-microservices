using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}
