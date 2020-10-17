using System;
using Convey.CQRS.Events;
using Convey.MessageBrokers;

namespace Trill.Services.Stories.Application.Events.External
{
    [Message("users")]
    public class UserUnlocked : IEvent
    {
        public Guid UserId { get; }

        public UserUnlocked(Guid userId)
        {
            UserId = userId;
        }
    }
}