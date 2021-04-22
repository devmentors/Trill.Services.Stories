using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Events;
using Trill.Services.Stories.Core.Factories;
using Trill.Services.Stories.Core.Policies;
using Trill.Services.Stories.Core.Repositories;
using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Application.Commands.Handlers
{
    internal sealed class SendStoryHandler : ICommandHandler<SendStory>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryTextFactory _storyTextFactory;
        private readonly IStoryAuthorPolicy _storyAuthorPolicy;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IStoryRequestStorage _storyRequestStorage;
        private readonly IEventMapper _eventMapper;
        private readonly IMessageBroker _messageBroker;

        public SendStoryHandler(IUserRepository userRepository, IStoryRepository storyRepository,
            IStoryTextFactory storyTextFactory, IStoryAuthorPolicy storyAuthorPolicy,
            IDomainEventDispatcher domainEventDispatcher, IClock clock, IIdGenerator idGenerator,
            IStoryRequestStorage storyRequestStorage, IEventMapper eventMapper, IMessageBroker messageBroker)
        {
            _userRepository = userRepository;
            _storyRepository = storyRepository;
            _storyTextFactory = storyTextFactory;
            _storyAuthorPolicy = storyAuthorPolicy;
            _domainEventDispatcher = domainEventDispatcher;
            _clock = clock;
            _idGenerator = idGenerator;
            _storyRequestStorage = storyRequestStorage;
            _eventMapper = eventMapper;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(SendStory command)
        {
            var user = await _userRepository.GetAsync(command.UserId);
            if (user is null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            if (!_storyAuthorPolicy.CanCreate(user))
            {
                throw new CannotCreateStoryException(user.Id);
            }
            
            var author = Author.Create(user);
            var text = _storyTextFactory.Create(command.Text);
            var now = _clock.Current();
            var visibility = command.VisibleFrom.HasValue && command.VisibleTo.HasValue
                ? new Visibility(command.VisibleFrom.Value, command.VisibleTo.Value, command.Highlighted)
                : Visibility.Default(now);
            var storyId = command.StoryId == default ? _idGenerator.Generate() : command.StoryId;
            var story = Story.Create(storyId, author, command.Title, text, command.Tags, now, visibility);
            await _storyRepository.AddAsync(story);
            var domainEvents = story.Events.ToArray();
            await _domainEventDispatcher.DispatchAsync(domainEvents);
            var integrationEvents = _eventMapper.Map(domainEvents).ToArray();
            _storyRequestStorage.SetStoryId(command.Id, story.Id);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}