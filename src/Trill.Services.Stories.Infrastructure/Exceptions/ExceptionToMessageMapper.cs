using System;
using Convey.MessageBrokers.RabbitMQ;
using Trill.Services.Stories.Application.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Exceptions;

namespace Trill.Services.Stories.Infrastructure.Exceptions
{
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message)
            => exception switch
            {
                AppException ex => new StoryActionRejected(ex.Message, ex.GetExceptionCode()),
                DomainException ex => new StoryActionRejected(ex.Message, ex.GetExceptionCode()),
                _ => new StoryActionRejected("There was an error", "story_error")
            };
    }
}

