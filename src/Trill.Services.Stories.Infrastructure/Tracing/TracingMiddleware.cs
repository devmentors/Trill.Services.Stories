using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Tag;

namespace Trill.Services.Stories.Infrastructure.Tracing
{
    internal  class TracingMiddleware : IMiddleware
    {
        private readonly ITracer _tracer;

        public TracingMiddleware(ITracer tracer)
            => _tracer = tracer;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using var scope = BuildScope(context);
            var span = scope.Span;

            try
            {
                span.Log("Starting handling HTTP request....");
                await next(context);
                span.Log("Finishing handling HTTP request....");
            }
            catch
            {
                span.Log("There was an error.");
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        private IScope BuildScope(HttpContext context)
        {
            var scope = _tracer
                .BuildSpan($"handling HTTP request: ${context.Request.Path}...")
                .WithTag(Tags.HttpMethod, context.Request.Method);

            if (_tracer.ActiveSpan is not null)
            {
                scope.AddReference(References.ChildOf, _tracer.ActiveSpan.Context);
            }

            return scope.StartActive(true);
        }
    }
}