using System.Threading.Tasks;
using Convey.CQRS.Events;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Entities;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.External.Handlers
{
    internal sealed class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserCreatedHandler(IUserRepository userRepository, IDateTimeProvider dateTimeProvider)
        {
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task HandleAsync(UserCreated @event)
        {
            var user = new User(@event.UserId, @event.Name, _dateTimeProvider.Now);
            await _userRepository.AddAsync(user);
        }
    }
}