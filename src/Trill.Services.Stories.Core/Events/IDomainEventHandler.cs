using System.Threading.Tasks;

namespace Trill.Services.Stories.Core.Events
{
    public interface IDomainEventHandler<in T> where T : class, IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}