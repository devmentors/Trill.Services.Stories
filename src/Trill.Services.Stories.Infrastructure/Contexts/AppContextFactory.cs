using System.Text.Json;
using System.Text.Json.Serialization;
using Convey.MessageBrokers;
using Microsoft.AspNetCore.Http;
using Trill.Services.Stories.Application;

namespace Trill.Services.Stories.Infrastructure.Contexts
{
    internal sealed class AppContextFactory : IAppContextFactory
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {new JsonStringEnumConverter()}
        };

        private readonly ICorrelationContextAccessor _contextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppContextFactory(ICorrelationContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = contextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public IAppContext Create()
        {
            if (_contextAccessor.CorrelationContext is { })
            {
                var payload = JsonSerializer.Serialize(_contextAccessor.CorrelationContext, SerializerOptions);

                return string.IsNullOrWhiteSpace(payload)
                    ? AppContext.Empty
                    : new AppContext(JsonSerializer.Deserialize<CorrelationContext>(payload, SerializerOptions));
            }

            var context = _httpContextAccessor.GetCorrelationContext();

            return context is null ? AppContext.Empty : new AppContext(context);
        }
    }
}