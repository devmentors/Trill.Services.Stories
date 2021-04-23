using System.Runtime.CompilerServices;
using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Microsoft.Extensions.DependencyInjection;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Factories;
using Trill.Services.Stories.Core.Policies;
using Trill.Services.Stories.Core.Services;

[assembly: InternalsVisibleTo("Trill.Services.Stories.Tests.Unit")]
[assembly: InternalsVisibleTo("Trill.Services.Stories.Tests.Integration")]

namespace Trill.Services.Stories.Application
{
    public static class Extensions
    {
        public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        {
            builder.Services
                .AddScoped<IStoryRatingService, StoryRatingService>()
                .AddSingleton<IStoryTextFactory, StoryTextFactory>()
                .AddSingleton<IStoryAuthorPolicy, StoryAuthorPolicy>();

            return builder
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher();
        }
    }
}