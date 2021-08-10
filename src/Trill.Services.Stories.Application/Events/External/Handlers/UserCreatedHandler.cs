using System.Threading.Tasks;
using Convey.CQRS.Events;

namespace Trill.Services.Stories.Application.Events.External.Handlers
{
    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        public async Task HandleAsync(UserCreated @event)
        {
            await Task.CompletedTask;
        }
    }
}