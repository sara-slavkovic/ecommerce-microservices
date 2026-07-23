using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.DTOs
{
    public class UserSnapshotDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
