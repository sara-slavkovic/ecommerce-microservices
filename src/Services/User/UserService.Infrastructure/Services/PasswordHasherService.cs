using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string HashPassword(string password)
        {
            // Pass null for the user object since it's not strictly required for basic hashing
            return _hasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);

            // Returns Success if the password matches
            return result == PasswordVerificationResult.Success;
        }
    }
}
