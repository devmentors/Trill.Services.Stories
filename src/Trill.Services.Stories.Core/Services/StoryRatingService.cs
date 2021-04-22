using System.Threading.Tasks;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Exceptions;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Core.Services
{
    public class StoryRatingService : IStoryRatingService
    {
        private readonly IStoryRatingRepository _storyRatingRepository;

        public StoryRatingService(IStoryRatingRepository storyRatingRepository)
        {
            _storyRatingRepository = storyRatingRepository;
        }
        
        public async Task<StoryRating> RateAsync(Story story, User user, int rate)
        {
            if (user.Locked)
            {
                throw new UserLockedException(user.Id);
            }
            
            var totalRating = await _storyRatingRepository.GetTotalRatingAsync(story.Id);
            totalRating += rate;
            var rating = StoryRating.Create(story.Id, user.Id, rate, totalRating);

            return rating;
        }
    }
}