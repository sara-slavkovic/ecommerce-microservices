using FluentValidation;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IValidator<RegisterUserDto> _registerValidator;
        private readonly IValidator<LoginUserDto> _loginValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IValidator<RegisterUserDto> registerValidator, IValidator<LoginUserDto> loginValidator, IValidator<UpdateUserDto> updateValidator)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _updateValidator = updateValidator;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var exists = await _userRepository.ExistsByUsernameAsync(dto.Username);
            if (exists)
                throw new ConflictException("Username is already taken.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = _passwordHasherService.HashPassword(dto.Password),
                FullName = dto.FullName,
                Phone = dto.Phone,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto> LoginAsync(LoginUserDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                throw new BadRequestException("Invalid username or password.");

            if (!user.IsActive)
                throw new BadRequestException("User account is not active.");

            var passwordValid = _passwordHasherService.VerifyPassword(user.PasswordHash, dto.Password);
            if (!passwordValid)
                throw new BadRequestException("Invalid username or password.");

            return MapToDto(user);
        }

        public async Task<UserSnapshotDto?> GetUserSnapshotByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new UserSnapshotDto
            {
                Id = user.Id,
                IsActive = user.IsActive,
                FullName = user.FullName,
                Phone = user.Phone
            };
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                var usernameTaken = await _userRepository.ExistsByUsernameExcludingIdAsync(dto.Username, id);
                if (usernameTaken)
                    throw new ConflictException("Username is already taken.");

                user.Username = dto.Username;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = _passwordHasherService.HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            user.UpdatedAt = DateTime.Now;

            await _userRepository.SaveChangesAsync();

            return MapToDto(user);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
