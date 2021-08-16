using System;
using System.Threading.Tasks;
using Convey;
using Convey.CQRS.Commands;
using Convey.Types;
using Prometheus;
using Trill.Services.Stories.Application.Exceptions;
using Trill.Services.Stories.Core.Exceptions;

namespace Trill.Services.Stories.Infrastructure.Decorators
{
    [Decorator]
    internal sealed class MetricsCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private static readonly Counter CommandRequests =
            Prometheus.Metrics.CreateCounter("command_requests", "Number of command executions.", "command", "service");
        private readonly ICommandHandler<TCommand> _handler;
        private readonly string _service;

        public MetricsCommandHandlerDecorator(ICommandHandler<TCommand> handler, AppOptions appOptions)
        {
            _handler = handler;
            _service = appOptions.Service;
        }

        public async Task HandleAsync(TCommand command)
        {
            var commandName = typeof(TCommand).Name.Underscore();
            CommandRequests.WithLabels(commandName, _service).Inc();
            await _handler.HandleAsync(command);
        }
    }
}