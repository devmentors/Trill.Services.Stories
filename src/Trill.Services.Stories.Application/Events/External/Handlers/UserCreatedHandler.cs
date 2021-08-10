using System.Threading.Tasks;
using Convey.CQRS.Events;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.External.Handlers
{
    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;

        public UserCreatedHandler(IUserRepository userRepository, IClock clock)
        {
            _userRepository = userRepository;
            _clock = clock;
        }

        public async Task HandleAsync(UserCreated @event)
        {
            var user = new User(@event.UserId, @event.Name, _clock.Current());
            await _userRepository.AddAsync(user);
        }
    }
}