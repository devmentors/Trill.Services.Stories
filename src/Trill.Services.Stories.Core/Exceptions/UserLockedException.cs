using System;

namespace Trill.Services.Stories.Core.Exceptions
{
    internal class UserLockedException : DomainException
    {
        public Guid UserId { get; }

        public UserLockedException(Guid userId) : base($"User with ID: '{userId}' is locked.")
        {
            UserId = userId;
        }
    }
}