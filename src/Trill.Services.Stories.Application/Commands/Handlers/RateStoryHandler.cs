using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Trill.Services.Stories.Application.Clients;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Commands.Handlers
{
    internal sealed class RateStoryHandler : ICommandHandler<RateStory>
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryRatingRepository _storyRatingRepository;
        private readonly IUsersApiClient _usersApiClient;

        public RateStoryHandler(IStoryRepository storyRepository, IStoryRatingRepository storyRatingRepository,
            IUsersApiClient usersApiClient)
        {
            _storyRepository = storyRepository;
            _storyRatingRepository = storyRatingRepository;
            _usersApiClient = usersApiClient;
        }

        public async Task HandleAsync(RateStory command)
        {
            var userDto = await _usersApiClient.GetAsync(command.UserId);
            if (userDto is null)
            {
                throw new UserNotFoundException(command.UserId);
            }
            
            var story = await _storyRepository.GetAsync(command.StoryId);
            if (story is null)
            {
                throw new StoryNotFoundException(command.StoryId);
            }

            await _storyRatingRepository.SetAsync(new StoryRating(new StoryRatingId(command.StoryId, command.UserId),
                command.Rate));
        }
    }
}