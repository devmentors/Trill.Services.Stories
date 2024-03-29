using Trill.Services.Stories.Core.Events;
using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Core.Entities
{
    public class StoryRating : AggregateRoot<StoryRatingId>
    {
        public Rate Rate { get; }

        public StoryRating(StoryRatingId id, Rate rate, int version = 0) : base(id, version)
        {
            Rate = rate;
        }

        public static StoryRating Create(StoryId storyId, UserId userId, int rate, int totalRate)
        {
            var rating = new StoryRating(new StoryRatingId(storyId, userId), new Rate(rate));
            rating.AddEvent(new StoryRatingChanged(rating, totalRate));

            return rating;
        }
    }
}