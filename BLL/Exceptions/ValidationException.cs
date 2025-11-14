using System;

namespace BLL.Exceptions
{
    public class ValidationException : LibraryException
    {
        public ValidationException(string message) : base(message) { }
    }
}