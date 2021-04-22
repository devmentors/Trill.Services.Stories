using Trill.Services.Stories.Core.Exceptions;
using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Core.Factories
{
    public class StoryTextFactory : IStoryTextFactory
    {
        public StoryText Create(string text)
        {
            if (text is null)
            {
                throw new InvalidStoryTextException();
            }
            
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new EmptyStoryTextException();
            }

            if (text.Length < 10)
            {
                throw new TooShortStoryTextException(text);
            }

            if (text.Length > 200)
            {
                throw new TooLongStoryTextException($"{text.Substring(200)}...");
            }

            return new StoryText(text.Trim());
        }
    }
}