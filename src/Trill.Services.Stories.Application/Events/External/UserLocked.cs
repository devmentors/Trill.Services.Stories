using System;
using Convey.CQRS.Events;
using Convey.MessageBrokers;

namespace Trill.Services.Stories.Application.Events.External
{
    [Message("users")]
    public class UserLocked : IEvent
    {
        public Guid UserId { get; }

        public UserLocked(Guid userId)
        {
            UserId = userId;
        }
    }
}