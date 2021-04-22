using System.Threading.Tasks;
using Trill.Services.Stories.Core.Events;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.Domain.Handlers
{
    internal sealed class StoryCreatedHandler : IDomainEventHandler<StoryCreated>
    {
        private readonly IUserRepository _userRepository;

        public StoryCreatedHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task HandleAsync(StoryCreated domainEvent)
        {
            await Task.CompletedTask;
        }
    }
}