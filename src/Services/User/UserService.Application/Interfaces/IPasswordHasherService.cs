using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
