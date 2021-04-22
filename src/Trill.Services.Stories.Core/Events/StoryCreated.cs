using Trill.Services.Stories.Core.Entities;

namespace Trill.Services.Stories.Core.Events
{
    public class StoryCreated : IDomainEvent
    {
        public Story Story { get; }

        public StoryCreated(Story story)
        {
            Story = story;
        }
    }
}