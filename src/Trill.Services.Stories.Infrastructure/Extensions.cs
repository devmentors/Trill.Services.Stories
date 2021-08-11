using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convey;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.MessageBrokers;
using Convey.Persistence.MongoDB;
using Convey.Persistence.Redis;
using Convey.Security;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Security;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.MessageBrokers.RabbitMQ.Serializers;
using Convey.Tracing.Jaeger;
using Trill.Services.Stories.Application;
using Trill.Services.Stories.Application.Clients;
using Trill.Services.Stories.Application.Commands;
using Trill.Services.Stories.Application.Events.External;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Core.Events;
using Trill.Services.Stories.Core.Repositories;
using Trill.Services.Stories.Infrastructure.Clients;
using Trill.Services.Stories.Infrastructure.Contexts;
using Trill.Services.Stories.Infrastructure.Decorators;
using Trill.Services.Stories.Infrastructure.Exceptions;
using Trill.Services.Stories.Infrastructure.Kernel;
using Trill.Services.Stories.Infrastructure.Mongo;
using Trill.Services.Stories.Infrastructure.Mongo.Documents;
using Trill.Services.Stories.Infrastructure.Mongo.Repositories;
using Trill.Services.Stories.Infrastructure.Protos;
using Trill.Services.Stories.Infrastructure.Services;

namespace Trill.Services.Stories.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services
                .AddTransient<IUsersApiClient, UsersApiHttpClient>()
                .AddSingleton<IRequestStorage, RequestStorage>()
                .AddSingleton<IStoryRequestStorage, StoryRequestStorage>()
                .AddSingleton<IIdGenerator, IdGenerator>()
                .AddSingleton<IClock, UtcClock>()
                .AddScoped<IMessageBroker, MessageBroker>()
                .AddScoped<IStoryRepository, StoryMongoRepository>()
                .AddScoped<IStoryRatingRepository, StoryRatingMongoRepository>()
                .AddScoped<IUserRepository, UserMongoRepository>()
                .AddTransient<IAppContextFactory, AppContextFactory>()
                .AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create())
                .AddDomainEvents()
                .AddGrpc();
            
             builder
                .AddErrorHandler<ExceptionToResponseMapper>()
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher()
                .AddHttpClient()
                .AddMongo()
                .AddRedis()
                .AddJaeger()
                .AddRabbitMq(serializer: new NewtonsoftJsonRabbitMqSerializer())
                .AddMessageOutbox(o => o.AddMongo())
                .AddExceptionToFailedMessageMapper<ExceptionToFailedMessageMapper>()
                .AddMongoRepository<StoryDocument, long>("stories")
                .AddMongoRepository<UserDocument, Guid>("users")
                .AddWebApiSwaggerDocs()
                .AddCertificateAuthentication()
                .AddSecurity();
             
             builder.Services.AddSingleton<ICorrelationIdFactory, CorrelationIdFactory>();
             builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
             builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));

             return builder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
                .UseSwaggerDocs()
                .UseConvey()
                .UseMongo()
                .UsePublicContracts<ContractAttribute>()
                .UseCertificateAuthentication()
                .UseRabbitMq()
                .SubscribeCommand<SendStory>()
                .SubscribeCommand<RateStory>()
                .SubscribeEvent<UserCreated>()
                .SubscribeEvent<UserLocked>()
                .SubscribeEvent<UserUnlocked>();

            app.UseRouting()
                .UseEndpoints(e => e.MapGrpcService<StoryServiceGrpcServer>());

            return app;
        }
        
        private static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }
        
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

        public static Task GetAppName(this HttpContext httpContext)
            => httpContext.Response.WriteAsync(httpContext.RequestServices.GetService<AppOptions>().Name);

        internal static CorrelationContext GetCorrelationContext(this IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext is null)
            {
                return null;
            }

            if (!accessor.HttpContext.Request.Headers.TryGetValue("Correlation-Context", out var json))
            {
                return null;
            }

            var value = json.FirstOrDefault();

            return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<CorrelationContext>(value);
        }

        internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
        {
            if (messageProperties is null)
            {
                return string.Empty;
            }

            if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
            {
                return Encoding.UTF8.GetString(spanBytes);
            }

            return string.Empty;
        }
    }
}