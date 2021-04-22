using Trill.Services.Stories.Core.Entities;

namespace Trill.Services.Stories.Core.Events
{
    public class StoryRatingChanged : IDomainEvent
    {
        public StoryRating Rating { get; }
        public int TotalRate { get; } 

        public StoryRatingChanged(StoryRating rating, int totalRate)
        {
            Rating = rating;
            TotalRate = totalRate;
        }
    }
}