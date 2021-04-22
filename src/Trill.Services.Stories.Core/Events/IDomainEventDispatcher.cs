using System.Threading.Tasks;

namespace Trill.Services.Stories.Core.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}