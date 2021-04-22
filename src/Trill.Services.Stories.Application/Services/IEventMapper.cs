using System.Collections.Generic;
using Convey.CQRS.Events;
using Trill.Services.Stories.Core.Events;

namespace Trill.Services.Stories.Application.Services
{
    public interface IEventMapper
    {
        IEnumerable<IEvent> Map(params IDomainEvent[] events);
    }
}