using System.Threading.Tasks;
using Convey.CQRS.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.External.Handlers
{
    internal sealed class UserLockedHandler : IEventHandler<UserLocked>
    {
        private readonly IUserRepository _userRepository;

        public UserLockedHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserLocked @event)
        {
            var user = await _userRepository.GetAsync(@event.UserId);
            if (user is null)
            {
                throw new UserNotFoundException(@event.UserId);
            }
                
            user.Lock();
            await _userRepository.UpdateAsync(user);
        }
    }
}