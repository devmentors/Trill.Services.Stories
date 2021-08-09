using System;
using Convey.CQRS.Commands;

namespace Trill.Services.Stories.Application.Commands
{
    public class RateStory : ICommand
    {
        public long StoryId { get; }
        public Guid UserId { get; }
        public int Rate { get; }

        public RateStory(long storyId, Guid userId, int rate)
        {
            StoryId = storyId;
            UserId = userId;
            Rate = rate;
        }
    }
}