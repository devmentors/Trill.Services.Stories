using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Trill.Services.Stories.Application.Clients;
using Trill.Services.Stories.Application.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Commands.Handlers
{
    internal sealed class RateStoryHandler : ICommandHandler<RateStory>
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryRatingRepository _storyRatingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageBroker _messageBroker;

        public RateStoryHandler(IStoryRepository storyRepository, IStoryRatingRepository storyRatingRepository,
            IUserRepository userRepository, IMessageBroker messageBroker)
        {
            _storyRepository = storyRepository;
            _storyRatingRepository = storyRatingRepository;
            _userRepository = userRepository;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(RateStory command)
        {
            var user = await _userRepository.GetAsync(command.UserId);
            if (user is null)
            {
                throw new UserNotFoundException(command.UserId);
            }
            
            if (user.Locked)
            {
                throw new UserLockedException(command.UserId);
            }
            
            var story = await _storyRepository.GetAsync(command.StoryId);
            if (story is null)
            {
                throw new StoryNotFoundException(command.StoryId);
            }

            await _storyRatingRepository.SetAsync(new StoryRating(new StoryRatingId(command.StoryId, command.UserId),
                command.Rate));
            var totalRating = await _storyRatingRepository.GetTotalRatingAsync(story.Id);
            await _messageBroker.PublishAsync(new StoryRated(command.StoryId, command.UserId,
                command.Rate, totalRating));
        }
    }
}