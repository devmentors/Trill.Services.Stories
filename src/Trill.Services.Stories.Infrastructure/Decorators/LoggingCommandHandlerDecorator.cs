using System.Threading.Tasks;
using Convey;
using Convey.CQRS.Commands;
using Convey.HTTP;
using Convey.Types;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Trill.Services.Stories.Infrastructure.Decorators
{
    [Decorator]
    internal sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> 
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly ICorrelationIdFactory _correlationIdFactory;
        private readonly ILogger<ICommandHandler<TCommand>> _logger;

        public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> commandHandler, 
            ICorrelationIdFactory correlationIdFactory, ILogger<ICommandHandler<TCommand>> logger)
        {
            _commandHandler = commandHandler;
            _correlationIdFactory = correlationIdFactory;
            _logger = logger;
        }

        public async Task HandleAsync(TCommand command)
        {
            var correlationId = _correlationIdFactory.Create();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                var name = command.GetType().Name.Underscore();
                _logger.LogInformation($"Handling a command {name}...");
                await _commandHandler.HandleAsync(command);
            }
            
        }
    }
}