using Trill.Services.Stories.Core.Entities;

namespace Trill.Services.Stories.Core.Policies
{
    public class StoryAuthorPolicy : IStoryAuthorPolicy
    {
        public bool CanCreate(User user) => !user.Locked && user.Rating >= -10;
    }
}