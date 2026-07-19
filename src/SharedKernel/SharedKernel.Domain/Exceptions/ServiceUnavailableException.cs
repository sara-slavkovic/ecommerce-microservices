using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Domain.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message) { }
    }
}
