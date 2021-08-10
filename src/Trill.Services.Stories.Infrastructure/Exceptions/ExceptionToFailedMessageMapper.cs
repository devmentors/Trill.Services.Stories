using System;
using Convey.MessageBrokers.RabbitMQ;
using Trill.Services.Stories.Application.Events;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Exceptions;

namespace Trill.Services.Stories.Infrastructure.Exceptions
{
    internal class ExceptionToFailedMessageMapper : IExceptionToFailedMessageMapper
    {
        public FailedMessage Map(Exception exception, object message)
            => exception switch
            {
                AppException ex => new FailedMessage(new StoryActionRejected(ex.Message, ex.GetExceptionCode()), false),
                DomainException ex => new FailedMessage(new StoryActionRejected(ex.Message, ex.GetExceptionCode()), false),
                _ => new FailedMessage(new StoryActionRejected("There was an error", "story_error"))
            };
    }
}