using System;

namespace BLL.Exceptions
{
    public class EntityNotFoundException : LibraryException
    {
        public EntityNotFoundException(string message) : base(message) { }
    }
}