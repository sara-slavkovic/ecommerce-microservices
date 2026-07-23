using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.DTOs
{
    public class UserSnapshotDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
