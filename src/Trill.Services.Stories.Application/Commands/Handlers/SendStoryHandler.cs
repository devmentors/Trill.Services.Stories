using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Factories;
using Trill.Services.Stories.Core.Repositories;
using Trill.Services.Stories.Core.ValueObjects;

namespace Trill.Services.Stories.Application.Commands.Handlers
{
    public sealed class SendStoryHandler : ICommandHandler<SendStory>
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryTextFactory _storyTextFactory;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IStoryRequestStorage _storyRequestStorage;

        public SendStoryHandler(IStoryRepository storyRepository, IStoryTextFactory storyTextFactory,
            IClock clock, IIdGenerator idGenerator, IStoryRequestStorage storyRequestStorage)
        {
            _storyRepository = storyRepository;
            _storyTextFactory = storyTextFactory;
            _clock = clock;
            _idGenerator = idGenerator;
            _storyRequestStorage = storyRequestStorage;
        }

        public async Task HandleAsync(SendStory command)
        {
            var author = Author.Create(command.UserId, $"user-{command.UserId:N}"); // Non-existent user for now
            var text = _storyTextFactory.Create(command.Text);
            var now = _clock.Current();
            var visibility = command.VisibleFrom.HasValue && command.VisibleTo.HasValue
                ? new Visibility(command.VisibleFrom.Value, command.VisibleTo.Value, command.Highlighted)
                : Visibility.Default(now);
            var storyId = command.StoryId <= 0 ? _idGenerator.Generate() : command.StoryId;
            var story = Story.Create(storyId, author, command.Title, text, command.Tags, now, visibility);
            await _storyRepository.AddAsync(story);
            _storyRequestStorage.SetStoryId(command.Id, story.Id);
        }
    }
}