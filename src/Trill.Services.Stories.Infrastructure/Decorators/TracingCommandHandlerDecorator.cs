using System;
using System.Threading.Tasks;
using Convey;
using Convey.CQRS.Commands;
using Convey.HTTP;
using Convey.Types;
using OpenTracing;
using OpenTracing.Tag;

namespace Trill.Services.Stories.Infrastructure.Decorators
{
    [Decorator]
    internal sealed class TracingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly ITracer _tracer;
        private readonly ICorrelationIdFactory _correlationIdFactory;

        public TracingCommandHandlerDecorator(ICommandHandler<TCommand> handler, ITracer tracer,
            ICorrelationIdFactory correlationIdFactory)
        {
            _handler = handler;
            _tracer = tracer;
            _correlationIdFactory = correlationIdFactory;
        }

        public async Task HandleAsync(TCommand command)
        {
            var commandName = command.GetType().Name.Underscore();
            var correlationId = _correlationIdFactory.Create();
            using var scope = BuildScope(commandName, correlationId);
            var span = scope.Span;

            try
            {
                span.Log($"Handling a command: {commandName}");
                await _handler.HandleAsync(command);
                span.Log($"Handled a command: {commandName}");
            }
            catch (Exception ex)
            {
                span.Log($"There was an error when handling a command: {commandName}");
                span.Log(ex.Message);
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        private IScope BuildScope(string commandName, string correlationId)
        {
            var scope = _tracer
                .BuildSpan($"handling-{commandName}")
                .WithTag("command", commandName)
                .WithTag("correlationId", correlationId);

            if (_tracer.ActiveSpan is {})
            {
                scope.AddReference(References.ChildOf, _tracer.ActiveSpan.Context);
            }

            return scope.StartActive(true);
        }
    }
}