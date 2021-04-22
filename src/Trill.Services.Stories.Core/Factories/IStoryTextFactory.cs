using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Core.Factories
{
    public interface IStoryTextFactory
    {
        StoryText Create(string text);
    }
}