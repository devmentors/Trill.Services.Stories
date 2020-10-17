using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Trill.Services.Stories.Application.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Policies;
using Trill.Services.Stories.Core.Repositories;
using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Application.Commands.Handlers
{
    internal sealed class SendStoryHandler : ICommandHandler<SendStory>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryTextPolicy _storyTextPolicy;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStoryIdGenerator _storyIdGenerator;
        private readonly IStoryRequestStorage _storyRequestStorage;
        private readonly IMessageBroker _messageBroker;

        public SendStoryHandler(IUserRepository userRepository, IStoryRepository storyRepository,
            IStoryTextPolicy storyTextPolicy, IDateTimeProvider dateTimeProvider, IStoryIdGenerator storyIdGenerator,
            IStoryRequestStorage storyRequestStorage, IMessageBroker messageBroker)
        {
            _userRepository = userRepository;
            _storyRepository = storyRepository;
            _storyTextPolicy = storyTextPolicy;
            _dateTimeProvider = dateTimeProvider;
            _storyIdGenerator = storyIdGenerator;
            _storyRequestStorage = storyRequestStorage;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(SendStory command)
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

            var author = Author.Create(user);
            var storyText = new StoryText(command.Text);
            _storyTextPolicy.Verify(storyText);
            var now = _dateTimeProvider.Now;
            var visibility = command.VisibleFrom.HasValue && command.VisibleTo.HasValue
                ? new Visibility(command.VisibleFrom.Value, command.VisibleTo.Value, command.Highlighted)
                : Visibility.Default(now);
            var storyId = command.StoryId == default ? _storyIdGenerator.GenerateId() : command.StoryId;
            var story = new Story(storyId, author, command.Title, storyText, command.Tags, now, visibility);
            await _storyRepository.AddAsync(story);
            _storyRequestStorage.SetStoryId(command.Id, story.Id);
            await _messageBroker.PublishAsync(new StorySent(storyId,
                new StorySent.AuthorModel(author.Id, author.Name), story.Title, story.Tags, story.CreatedAt,
                new StorySent.VisibilityModel(story.Visibility.From, story.Visibility.To,
                    story.Visibility.Highlighted)));
        }
    }
}