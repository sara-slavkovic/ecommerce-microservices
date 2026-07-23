using UserService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            When(x => x.Username != null, () =>
            {
                RuleFor(x => x.Username)
                    .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                    .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                    .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, digits, and the characters - . _");
            });

            When(x => x.Password != null, () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                    .Matches(@"[!@#$%^&*(),.?"":{}|<>_\-+=\[\]\\/;'`~]").WithMessage("Password must contain at least one special character.");
            });

            When(x => x.Phone != null, () =>
            {
                RuleFor(x => x.Phone)
                    .Matches(@"^\+381\d{8,9}$").WithMessage("Phone number must start with +381 followed by 8 or 9 digits.");
            });

            When(x => x.FullName != null, () =>
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Full name cannot be empty.")
                    .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.");
            });
        }
    }
}
