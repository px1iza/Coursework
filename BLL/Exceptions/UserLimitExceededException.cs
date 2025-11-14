using System;

namespace BLL.Exceptions
{
    public class UserLimitExceededException : LibraryException
    {
        public UserLimitExceededException(string message) : base(message) { }
    }
}