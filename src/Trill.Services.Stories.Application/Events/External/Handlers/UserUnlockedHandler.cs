﻿using System.Threading.Tasks;
using Convey.CQRS.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Repositories;

namespace Trill.Services.Stories.Application.Events.External.Handlers
{
    public sealed class UserUnlockedHandler : IEventHandler<UserUnlocked>
    {
        private readonly IUserRepository _userRepository;

        public UserUnlockedHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserUnlocked @event)
        {
            var user = await _userRepository.GetAsync(@event.UserId);
            if (user is null)
            {
                throw new UserNotFoundException(@event.UserId);
            }
            
            user.Unlock();
            await _userRepository.UpdateAsync(user);
        }
    }
}