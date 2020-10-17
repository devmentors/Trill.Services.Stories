using System;
using System.Threading.Tasks;
using Convey.HTTP;
using Trill.Services.Stories.Application.Clients;
using Trill.Services.Stories.Application.Clients.DTO;

namespace Trill.Services.Stories.Infrastructure.Clients.HTTP
{
    internal sealed class UsersApiHttpClient : IUsersApiClient
    {
        private readonly IHttpClient _client;
        private readonly string _url;

        public UsersApiHttpClient(IHttpClient client, HttpClientOptions options)
        {
            _client = client;
            _url = options.Services["users"];
        }

        public Task<UserDto> GetAsync(Guid userId) => _client.GetAsync<UserDto>($"{_url}/users/{userId}");
    }
}