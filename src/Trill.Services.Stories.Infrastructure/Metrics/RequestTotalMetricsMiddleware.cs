using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.Types;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace Trill.Services.Stories.Infrastructure.Metrics
{
    internal sealed class RequestTotalMetricsMiddleware : IMiddleware
    {
        private static readonly ISet<string> IgnoredPaths = new HashSet<string>
        {
            "/metrics",
            "/ping",
            "/health"
        };
        
        private readonly AppOptions _appOptions;
        private readonly Counter _totalRequests;

        public RequestTotalMetricsMiddleware(AppOptions appOptions)
        {
            _appOptions = appOptions;
            _totalRequests = Prometheus.Metrics.CreateCounter("total_requests", "Number of HTTP requests.",
                "method", "service");
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (IgnoredPaths.Contains(context.Request.Path))
            {
                return next(context);
            }
            
            _totalRequests.WithLabels(context.Request.Method, _appOptions.Service).Inc();

            return next(context);
        }
    }
}