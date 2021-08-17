using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pactify;
using Trill.Services.Stories.Application.Clients.DTO;
using Xunit;

namespace Trill.Services.Stories.Tests.Contracts.Consumer
{
    public class UserServiceConsumerTests
    {
        private const string UserId = "0116012e-988c-4c3c-b1a9-3da8140e61c0";
        
        [Fact]
        public async Task given_valid_user_id_user_should_be_returned()
        {
            var options = new PactDefinitionOptions
            {
                IgnoreCasing = true,
                IgnoreContractValues = true
            };

            await PactMaker
                .Create(options)
                .Between("stories", "users")
                .WithHttpInteraction(b => b
                    .Given("Existing user")
                    .UponReceiving("A GET method to retrieve user details")
                    .With(request => request
                        .WithMethod(HttpMethod.Get)
                        .WithPath($"/users/{UserId}"))
                    .WillRespondWith(response => response
                        .WithHeader("Content-Type", "application/json")
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody<UserDto>()))
                .PublishedViaHttp("http://localhost:9292/pacts/provider/users/consumer/stories/version/1.2.104", HttpMethod.Put)
                .MakeAsync();
        }
    }
}