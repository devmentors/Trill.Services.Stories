using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Trill.Services.Stories.Application.Clients;
using Trill.Services.Stories.Application.Clients.DTO;

namespace Trill.Services.Stories.Infrastructure.Clients
{
    public class UsersApiHttpClient : IUsersApiClient
    {
        private const string Url = "http://localhost:5070";
        private readonly IHttpClientFactory _factory;

        public UsersApiHttpClient(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<UserDto> GetAsync(Guid userId)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"{Url}/users/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
    }
}