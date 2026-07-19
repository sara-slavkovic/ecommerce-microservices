using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Domain.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
