using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}
