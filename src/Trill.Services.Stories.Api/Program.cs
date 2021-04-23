using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Convey;
using Convey.Logging;
using Convey.Secrets.Vault;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trill.Services.Stories.Application;
using Trill.Services.Stories.Infrastructure;

[assembly: InternalsVisibleTo("Trill.Services.Stories.Tests.Unit")]
[assembly: InternalsVisibleTo("Trill.Services.Stories.Tests.Integration")]

namespace Trill.Services.Stories.Api
{
    internal static class Program
    {
        public static async Task Main(string[] args)
            => await CreateHostBuilder(args)
                .Build()
                .RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .ConfigureServices(services =>
                    {
                        services.AddControllers()
                            .AddJsonOptions(x =>
                            {
                                x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                            });
                        services
                            .AddConvey()
                            .AddWebApi()
                            .AddApplication()
                            .AddInfrastructure()
                            .Build();
                    })
                    .Configure(app => app
                        .UseInfrastructure()
                        .UseEndpoints(e => e.MapControllers())
                        .UseDispatcherEndpoints(endpoints => endpoints
                            .Get("", ctx => ctx.GetAppName())))
                    .ConfigureKestrel((_, k) =>
                    {
                        k.ListenLocalhost(5050, o => o.Protocols = HttpProtocols.Http1);
                        k.ListenLocalhost(5051, o => o.Protocols = HttpProtocols.Http2);
                    }))
                .UseLogging()
                .UseVault();
    }
}
