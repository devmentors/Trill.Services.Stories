using Trill.Services.Stories.Core.Entities;

namespace Trill.Services.Stories.Core.Policies
{
    public interface IStoryAuthorPolicy
    {
        bool CanCreate(User user);
    }
}