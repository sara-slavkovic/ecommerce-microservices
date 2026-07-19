using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
