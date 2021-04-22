using System;

namespace Trill.Services.Stories.Application.Exceptions
{
    internal class CannotCreateStoryException : AppException
    {
        public CannotCreateStoryException(Guid userId) : base($"Story cannot be created by user with ID: '{userId}'.")
        {
        }
    }
}