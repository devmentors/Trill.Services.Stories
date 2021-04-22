using System.Threading.Tasks;
using Trill.Services.Stories.Core.Entities;

namespace Trill.Services.Stories.Core.Services
{
    public interface IStoryRatingService
    {
        Task<StoryRating> RateAsync(Story story, User user, int rate);
    }
}