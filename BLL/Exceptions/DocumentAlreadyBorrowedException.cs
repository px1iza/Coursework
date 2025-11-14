using System;

namespace BLL.Exceptions
{
    public class DocumentAlreadyBorrowedException : LibraryException
    {
        public DocumentAlreadyBorrowedException(string message) : base(message) { }
    }
}