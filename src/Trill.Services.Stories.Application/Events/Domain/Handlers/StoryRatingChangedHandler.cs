using System.Threading.Tasks;
using Trill.Services.Stories.Core.Events;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.Domain.Handlers
{
    internal sealed class StoryRatingChangedHandler : IDomainEventHandler<StoryRatingChanged>
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IUserRepository _userRepository;

        public StoryRatingChangedHandler(IStoryRepository storyRepository, IUserRepository userRepository)
        {
            _storyRepository = storyRepository;
            _userRepository = userRepository;
        }
        
        public async Task HandleAsync(StoryRatingChanged domainEvent)
        {
            var story = await _storyRepository.GetAsync(domainEvent.Rating.Id.StoryId);
            var user = await _userRepository.GetAsync(story.Author.Id);
            user.AddRating(domainEvent.Rating.Rate);
            await _userRepository.UpdateAsync(user);
        }
    }
}